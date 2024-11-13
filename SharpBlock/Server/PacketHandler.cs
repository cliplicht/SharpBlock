using System.Buffers;
using System.IO.Compression;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SharpBlock.Options;
using SharpBlock.Pakets;
using SharpNBT.Extensions;

namespace SharpBlock.Server
{
    public class PacketHandler : IDisposable
    {
        private readonly ILogger _logger;
        private readonly ServerOptions _serverOptions;
        private readonly WorldsOptions _worldOptions;
        private readonly ClientConnection _clientConnection;
        private readonly EncryptionHandler _encryptionHandler;

        private readonly byte[] _receiveBuffer;
        private int _receiveBufferOffset = 0;
        private ConnectionState _connectionState = ConnectionState.Handshaking;

        private const int BufferSize = 8192; // Adjust the buffer size as needed

        private readonly PaketFactory _packetFactory = new();
        private byte[] _verifyToken = new byte[4];
        private readonly HttpClient _httpClient;
        private MinecraftAccount _account;

        public PacketHandler(
            ILogger logger,
            ServerOptions serverOptions,
            WorldsOptions worldOptions,
            ClientConnection clientConnection,
            EncryptionHandler encryptionHandler)
        {
            _logger = logger;
            _serverOptions = serverOptions;
            _worldOptions = worldOptions;
            _clientConnection = clientConnection;
            _encryptionHandler = encryptionHandler;
            _receiveBuffer = ArrayPool<byte>.Shared.Rent(BufferSize);
            _httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(5)
            };

            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(_verifyToken);
        }

        public void HandleReceivedData(byte[] buffer, int offset, int count)
        {
            // Copy the received data into the internal buffer
            Buffer.BlockCopy(buffer, offset, _receiveBuffer, _receiveBufferOffset, count);
            _receiveBufferOffset += count;

            // Process the received data
            ProcessReceivedData();
        }

        private void ProcessReceivedData()
        {
            int offset = 0;

            while (offset < _receiveBufferOffset)
            {
                // Attempt to read the packet length
                int packetLength;
                int lengthOfPacketLength;

                try
                {
                    packetLength = _receiveBuffer.ReadVarInt(offset, out lengthOfPacketLength);
                }
                catch (Exception)
                {
                    // Not enough data to read the packet length, wait for more data
                    break;
                }

                // Check if enough data is available for the entire packet
                if (_receiveBufferOffset - offset - lengthOfPacketLength < packetLength)
                {
                    // Not enough data, wait for more
                    break;
                }

                int dataStart = offset + lengthOfPacketLength;
                int dataLength = packetLength;

                ReadOnlySpan<byte> packetData = new ReadOnlySpan<byte>(_receiveBuffer, dataStart, dataLength);

                if (_serverOptions.Compression)
                {
                    int uncompressedDataLength;
                    int lengthOfUncompressedDataLength;
                    uncompressedDataLength = packetData.ReadVarInt(0, out lengthOfUncompressedDataLength);

                    if (uncompressedDataLength > 0)
                    {
                        // Packet is compressed
                        ReadOnlySpan<byte> compressedData = packetData.Slice(lengthOfUncompressedDataLength);
                        byte[] decompressedData = Decompress(compressedData.ToArray(), uncompressedDataLength);
                        packetData = new ReadOnlySpan<byte>(decompressedData);
                    }
                    else
                    {
                        // Packet is not compressed
                        packetData = packetData.Slice(lengthOfUncompressedDataLength);
                    }
                }

                // Read the Packet ID
                int packetIdLength;
                int packetId = packetData.ReadVarInt(0, out packetIdLength);

                // Packet data without Packet ID
                ReadOnlySpan<byte> actualPacketData = packetData.Slice(packetIdLength);

                // Handle the packet
                HandlePacket(packetId, actualPacketData);

                // Move the offset
                offset += lengthOfPacketLength + packetLength;
            }

            // Remove the processed data from the buffer
            if (offset > 0)
            {
                Buffer.BlockCopy(_receiveBuffer, offset, _receiveBuffer, 0, _receiveBufferOffset - offset);
                _receiveBufferOffset -= offset;
            }
        }

        private void HandlePacket(int packetId, ReadOnlySpan<byte> packetData)
        {
            try
            {
                if (_clientConnection.IsConnectionClosed)
                    return;

                _logger.LogDebug($"Handling packet with ID: {packetId} in state {_connectionState}");

                IPaket? packet = _packetFactory.CreatePaket(packetId, _connectionState);

                if (packet == null)
                {
                    _logger.LogWarning($"Unknown packet with ID: {packetId} in state {_connectionState}");
                    return;
                }

                using (var ms = new MemoryStream(packetData.ToArray()))
                {
                    packet.Read(ms);
                }

                packet.Handle(this);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception while handling packet ID {packetId}: {ex.Message}");
                _clientConnection.CloseConnection();
            }
        }

        public void SendPacket(IPaket packet)
        {
            if (_clientConnection.IsConnectionClosed)
            {
                _logger.LogWarning("Attempted to send packet after connection was closed.");
                return;
            }

            try
            {
                using (var ms = new MemoryStream())
                {
                    ms.WriteVarInt(packet.PacketId);
                    packet.Write(ms);
                    byte[] packetData = ms.ToArray();

                    byte[] finalPacket;

                    if (_serverOptions.Compression)
                    {
                        using (var compressedStream = new MemoryStream())
                        {
                            int dataLength = packetData.Length;
                            if (dataLength >= _serverOptions.CompressionThreshold)
                            {
                                byte[] compressedData = Compress(packetData);
                                compressedStream.WriteVarInt(dataLength);
                                compressedStream.Write(compressedData, 0, compressedData.Length);
                            }
                            else
                            {
                                compressedStream.WriteVarInt(0);
                                compressedStream.Write(packetData, 0, packetData.Length);
                            }

                            byte[] compressedPacketData = compressedStream.ToArray();
                            using (var finalStream = new MemoryStream())
                            {
                                finalStream.WriteVarInt(compressedPacketData.Length);
                                finalStream.Write(compressedPacketData, 0, compressedPacketData.Length);
                                finalPacket = finalStream.ToArray();
                            }
                        }
                    }
                    else
                    {
                        using (var finalStream = new MemoryStream())
                        {
                            // write packet length
                            finalStream.WriteVarInt(packetData.Length);
                            // write packet data
                            finalStream.Write(packetData, 0, packetData.Length);
                            finalPacket = finalStream.ToArray();
                        }
                    }

                    // send the final packet
                    _clientConnection.SendData(finalPacket);
                    _logger.LogDebug($"Sent packet ID {packet.PacketId} to client. Length: {finalPacket.Length}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception while sending packet ID {packet.PacketId}: {ex.Message}");
                _clientConnection.CloseConnection();
            }
        }

        public void HandleHandshake(HandshakePaket packet)
        {
            _logger.LogDebug(
                $"Received handshake: ProtocolVersion {packet.ProtocolVersion}, ServerAddress {packet.ServerAddress}, ServerPort {packet.ServerPort}, NextState {packet.NextState}");

            // Update the connection state based on NextState
            if (packet.NextState == 1)
            {
                _connectionState = ConnectionState.Status;
            }
            else if (packet.NextState == 2)
            {
                _connectionState = ConnectionState.Login;
            }
            else
            {
                _logger.LogWarning($"Unknown NextState: {packet.NextState}");
            }

            _logger.LogDebug($"Connection state updated to: {_connectionState}");
        }

        public void HandlePingRequest(PingRequestPaket packet)
        {
            _logger.LogDebug($"Received Ping Request with payload: {packet.Payload}");

            var pingResponsePaket = new PingResponsePaket()
            {
                Payload = packet.Payload
            };

            SendPacket(pingResponsePaket);
            _logger.LogDebug($"Ping Response sent to client.");
        }

        public void HandleStatusRequest(StatusRequestPaket packet)
        {
            _logger.LogDebug("Received Status Request");

            byte[] imageBytes;
            string serverImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "serverFaviconSmall.png");
            using (var image = new FileStream(serverImagePath, FileMode.Open, FileAccess.Read))
            {
                using (var ms = new MemoryStream())
                {
                    image.CopyTo(ms);
                    imageBytes = ms.ToArray();
                }
            }

            // Create the Status Response Packet
            var statusResponsePacket = new StatusResponsePaket()
            {
                Name = _serverOptions.Instance.VersionName,
                Protocol = _serverOptions.Instance.ProtocolVersion,
                Max = _serverOptions.MaxPlayers,
                Online = 0,
                Sample = [],
                Text = _serverOptions.Description,
                Favicon = "data:image/png;base64," + Convert.ToBase64String(imageBytes),
                EnforceSecureChat = false,
            };

            // Send the response
            SendPacket(statusResponsePacket);
            _logger.LogDebug("Status Response sent to client.");
        }

        public void HandleLoginStart(LoginStartPaket packet)
        {
            _logger.LogDebug($"Player {packet.PlayerName} is attempting to log in.");
            var id = Guid.NewGuid();
            _account = new MinecraftAccount(id, packet.PlayerName);

            if (_serverOptions is { OnlineMode: true, Encryption: true } or { OnlineMode: false, Encryption: true })
            {
                var encryptionPackage = new EncryptionRequestPacket()
                {
                    ServerId = "",
                    PublicKey = _encryptionHandler.PublicKey,
                    VerifyToken = _verifyToken,
                    ShouldAuthenticate = _serverOptions.Encryption
                };

                SendPacket(encryptionPackage);
            }
            else if (_serverOptions is { OnlineMode: false, Encryption: false })
            {
                if (_serverOptions.Compression)
                {
                    SendCompression();
                }

                SendLoginSuccess();
            }
        }

        private void SendCompression()
        {
            var setCompressionPacket = new SetCompressionPacket()
            {
                Threshold = _serverOptions.CompressionThreshold
            };

            SendPacket(setCompressionPacket);
        }

        private void SendLoginSuccess()
        {
            if (_clientConnection.IsConnectionClosed)
                return;

            try
            {
                var loginSuccessPacket = new LoginSuccessPacket()
                {
                    Id = _account.Id.ToString("N"),
                    Username = _account.Name,
                    Properties = _account.Properties
                };

                SendPacket(loginSuccessPacket);
                _logger.LogDebug("Login Success sent to client.");
                _connectionState = ConnectionState.Play;

                SendJoinGamePacket();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in SendLoginSuccess: {ex.Message}");
            }
        }

        private void SendJoinGamePacket()
        {
            if (_clientConnection.IsConnectionClosed)
                return;

            try
            {
                var joinGamePacket = new JoinGamePaket()
                {
                    WorldsOptions = _worldOptions,
                    EntityId = 1,
                    MaxPlayers = _serverOptions.MaxPlayers,
                    HashedSeed = 0,
                    PreviousGameMode = -1,
                };

                SendPacket(joinGamePacket);
                _logger.LogDebug("Join Game sent to client.");

                SendInitialPackets();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in SendJoinGamePacket: {ex.Message}");
            }
        }

        private void SendInitialPackets()
        {
            if (_clientConnection.IsConnectionClosed)
                return;

            try
            {
                var positionPacket = new PlayerPositionAndLookPacket()
                {
                    X = 0.0,
                    Y = 64.0,
                    Z = 0.0,
                    Yaw = 0.0f,
                    Pitch = 0.0f,
                    Flags = 0,
                    TeleportId = 0
                };

                SendPacket(positionPacket);

                var abilitiesPacket = new PlayerAbilitiesPacket()
                {
                    Flags = 0,
                    FlyingSpeed = 0.05f,
                    WalkingSpeed = 0.1f
                };

                SendPacket(abilitiesPacket);

                var heldItemChangePacket = new SetHeldItemPacket()
                {
                    Slot = 0
                };
                SendPacket(heldItemChangePacket);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in SendInitialPackets: {ex.Message}");
            }
        }

        public async void HandleEncryptionResponse(EncryptionResponsePacket packet)
        {
            _logger.LogDebug("Handling Encryption Response.");

            byte[] sharedSecret = _encryptionHandler.DecryptData(packet.EncryptedSharedSecret);
            byte[] verifyToken = _encryptionHandler.DecryptData(packet.EncryptedVerifyToken);

            if (!verifyToken.SequenceEqual(_verifyToken))
            {
                _logger.LogWarning("Encrypted Verify Token does not match Shared Secret");
                _clientConnection.CloseConnection();
                return;
            }

            _clientConnection.EnableEncryption(sharedSecret);
            _logger.LogDebug("Encryption enabled for client.");

            if (_serverOptions.OnlineMode)
            {
                if (!await AuthenticateWithMojangAsync(sharedSecret))
                {
                    _logger.LogWarning("Failed to authenticate with Mojang. Disconnecting client.");
                    _clientConnection.CloseConnection();
                    return;
                }
            }

            if (_serverOptions.Compression)
            {
                SendCompression();
            }

            SendLoginSuccess();
        }

        private async Task<bool> AuthenticateWithMojangAsync(byte[] sharedSecret)
        {
            _logger.LogDebug("Authenticating with Mojang.");

            string serverId = Utilities.Utilities.GenerateServerHash(sharedSecret, _encryptionHandler.PublicKey);
            var ipAddress = _clientConnection.GetClientIpAddress().Address;

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get,
                $"https://sessionserver.mojang.com/session/minecraft/hasJoined?username={_account.Name}&serverId={serverId}");
            request.Headers.Add("User-Agent", "SharpBlock Server");
            request.Headers.Add("Accept", "application/json");

            _logger.LogDebug("Sending request to Mojang for authentication. {request}", request.RequestUri);

            try
            {
                var response = await _httpClient.SendAsync(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var mcAccount = JsonSerializer.Deserialize<MinecraftAccount>(content);

                    if (mcAccount != null)
                    {
                        _account = mcAccount;
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception during Mojang authentication: {ex.Message}");
            }

            return false;
        }

        public void Dispose()
        {
            ArrayPool<byte>.Shared.Return(_receiveBuffer);
            _httpClient.Dispose();
        }

        private byte[] Compress(byte[] data)
        {
            using (var ms = new MemoryStream())
            {
                using (var deflateStream = new ZLibStream(ms, CompressionMode.Compress))
                {
                    deflateStream.Write(data, 0, data.Length);
                }

                return ms.ToArray();
            }
        }

        private byte[] Decompress(byte[] data, int uncompressedSize)
        {
            using (var ms = new MemoryStream(data))
            {
                using (var output = new MemoryStream())
                {
                    using (var deflateStream = new ZLibStream(ms, CompressionMode.Decompress))
                    {
                        deflateStream.CopyTo(output);
                    }
                    return output.ToArray();
                }
            }
        }
    }
}