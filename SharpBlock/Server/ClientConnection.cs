using System.Buffers;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SharpBlock.Options;

namespace SharpBlock.Server
{
    public class ClientConnection : IDisposable
    {
        private readonly ILogger _logger;
        private readonly ServerOptions _serverOptions;
        private readonly WorldsOptions _worldOptions;
        private readonly Socket _clientSocket;
        private readonly EncryptionHandler _encryptionHandler;
        private readonly PacketHandler _packetHandler;

        private NetworkStream _baseStream;
        private CryptoStream? _encryptedWriteStream;
        private ICryptoTransform? _decryptor;

        private readonly byte[] _receiveBuffer;
        private const int BufferSize = 8192; // Adjust the buffer size as needed

        private volatile bool _isConnectionClosed = false;
        public bool IsConnectionClosed => _isConnectionClosed;

        public ClientConnection(
            ILogger logger,
            ServerOptions serverOptions,
            WorldsOptions worldOptions,
            Socket clientSocket,
            EncryptionHandler encryptionHandler)
        {
            _logger = logger;
            _serverOptions = serverOptions;
            _worldOptions = worldOptions;
            _clientSocket = clientSocket;
            _encryptionHandler = encryptionHandler;

            // Initialize NetworkStream
            _baseStream = new NetworkStream(_clientSocket, ownsSocket: false);
            _packetHandler = new PacketHandler(_logger, _serverOptions, _worldOptions, this, _encryptionHandler);

            _receiveBuffer = ArrayPool<byte>.Shared.Rent(BufferSize);
        }

        public void StartHandling()
        {
            // Start receiving data asynchronously
            Task.Run(() => ReceiveDataAsync());
        }

        public void EnableEncryption(byte[] sharedSecret)
        {
            var aes = Aes.Create();
            aes.Key = sharedSecret;
            aes.IV = sharedSecret;
            aes.Mode = CipherMode.CFB;
            aes.Padding = PaddingMode.None;

            var encryptor = aes.CreateEncryptor();
            _decryptor = aes.CreateDecryptor();

            // Create CryptoStream for encryption
            _encryptedWriteStream = new CryptoStream(_baseStream, encryptor, CryptoStreamMode.Write, leaveOpen: true);

            _logger.LogInformation("Encryption enabled for client.");
        }

        private async Task ReceiveDataAsync()
        {
            try
            {
                while (!_isConnectionClosed)
                {
                    int bytesRead = await _baseStream.ReadAsync(_receiveBuffer, 0, BufferSize);
                    if (bytesRead > 0)
                    {
                        byte[] data = new byte[bytesRead];
                        Array.Copy(_receiveBuffer, data, bytesRead);

                        if (_decryptor != null)
                        {
                            // Decrypt the data manually
                            byte[] decryptedData = DecryptData(data);
                            // Pass the decrypted data to the PacketHandler
                            _packetHandler.HandleReceivedData(decryptedData, 0, decryptedData.Length);
                        }
                        else
                        {
                            // Pass the received data to the PacketHandler
                            _packetHandler.HandleReceivedData(data, 0, bytesRead);
                        }
                    }
                    else
                    {
                        // No more data; close the connection
                        CloseConnection();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception in ReceiveDataAsync: {ex.Message}");
                CloseConnection();
            }
        }

        private byte[] DecryptData(byte[] data)
        {
            using (var input = new MemoryStream(data))
            using (var cs = new CryptoStream(input, _decryptor!, CryptoStreamMode.Read))
            using (var output = new MemoryStream())
            {
                cs.CopyTo(output);
                return output.ToArray();
            }
        }

        public void SendData(byte[] data)
        {
            if (_isConnectionClosed)
            {
                _logger.LogWarning("Attempted to send data after connection was closed.");
                return;
            }

            try
            {
                if (_encryptedWriteStream != null)
                {
                    _encryptedWriteStream.Write(data, 0, data.Length);
                    _encryptedWriteStream.Flush(); // Ensure data is flushed
                }
                else
                {
                    _baseStream.Write(data, 0, data.Length);
                    _baseStream.Flush();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending data: {ex.Message}");
                CloseConnection();
            }
        }

        public void CloseConnection()
        {
            if (_isConnectionClosed)
                return;

            _isConnectionClosed = true;

            try
            {
                _clientSocket.Shutdown(SocketShutdown.Both);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error shutting down client socket: {ex.Message}");
            }

            _encryptedWriteStream?.Dispose();
            _baseStream.Dispose();
            _clientSocket.Close();

            Dispose(); // Dispose ClientConnection
        }

        public IPEndPoint GetClientIpAddress()
        {
            return (_clientSocket.RemoteEndPoint as IPEndPoint)!;
        }

        public void Dispose()
        {
            if (_isConnectionClosed)
                return;

            _encryptedWriteStream?.Dispose();
            _baseStream.Dispose();
            _clientSocket.Dispose();
            ArrayPool<byte>.Shared.Return(_receiveBuffer);
            _isConnectionClosed = true;
        }
    }
}