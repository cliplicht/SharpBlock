using System.Net;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic.CompilerServices;
using SharpBlock.Core;
using SharpBlock.Core.Extensions;
using SharpBlock.Core.Models;
using SharpBlock.Core.Network;
using SharpBlock.Core.Options;
using SharpBlock.Core.Protocol;
using SharpBlock.Core.Services;
using SharpBlock.Protocol.Packets.Compression;
using SharpBlock.Protocol.Packets.Configuration;
using SharpBlock.Protocol.Packets.Encryption;
using SharpBlock.Protocol.Packets.Login;
using SharpBlock.Protocol.Packets.Ping;
using SharpBlock.Protocol.Packets.Play;
using SharpBlock.Protocol.Packets.Status;
using Utils = SharpBlock.Core.Utils.Utils;
using Version = SharpBlock.Core.Models.Version;

namespace SharpBlock.Protocol.Handlers;

public class PacketHandler : IPacketHandler
{
    private readonly ILogger<PacketHandler> _logger;
    private readonly IOptions<ServerOptions> _serverOptions;
    private readonly EncryptionService _encryptionService;
    private IClientConnection? _clientConnection;

    private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
        { PropertyNamingPolicy = JsonNamingPolicy.KebabCaseLower };

    public PacketHandler(ILogger<PacketHandler> logger, IOptions<ServerOptions> serverOptions,
        EncryptionService encryptionService)
    {
        _logger = logger;
        _serverOptions = serverOptions;
        _encryptionService = encryptionService;
    }

    public Task SetClientConnectionAsync(IClientConnection clientConnection, CancellationToken token = default)
    {
        if (!token.IsCancellationRequested)
        {
            _clientConnection = clientConnection;
        }

        return Task.CompletedTask;
    }

    public Task HandleKeepAliveAsync(IKeepAlivePacket packet, CancellationToken token = default)
    {
        _logger.LogDebug("Received KeepAlive packet with ID {KeepAliveId}", packet.KeepAliveId);

        // Optionally send a response to acknowledge the KeepAlive packet.
        if (_clientConnection != null)
        {
            _clientConnection.SendPacketAsync(packet);
        }

        return Task.CompletedTask;
    }

    public async Task HandleClientSettingsAsync(IClientSettingsPacket packet, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Received Client Settings: Locale={Locale}, ViewDistance={ViewDistance}, ChatMode={ChatMode}, ChatColors={ChatColors}, SkinParts={DisplayedSkinParts}, MainHand={MainHand}",
            packet.Locale, packet.ViewDistance, packet.ChatMode, packet.ChatColors, packet.DisplayedSkinParts,
            packet.MainHand);


        var configFinish = new FinishConfigurationPacket();
        await _clientConnection.SendPacketAsync(configFinish);
    }

    public Task HandlePluginMessageAsync(IPluginMessagePacket packet, CancellationToken token = default)
    {
        _logger.LogInformation("Received PluginMessage on channel {Channel} with data length {DataLength}", packet.Channel, packet.Data.Length);
    
        // Handle specific channels if necessary
        if (packet.Channel.StartsWith("minecraft:"))
        {
            // Special handling for Minecraft channels
        }

        return Task.CompletedTask;
    }

    public async Task HandleKnownPacksAsync(IKnownPacksResponse packet, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }

    public async Task HandleAcknowledgeFinishConfigurationAsync(IPacket packet, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Received AcknowledgeFinishConfigurationPacket");
        _clientConnection.ConnectionState = ConnectionState.Play;
        
        //TODO: send JoinGamePacket
        
        
        //await _clientConnection.SendPacketAsync(null);
        await Task.CompletedTask;
    }

    public async Task HandlePacketAsync(IPacket packet, CancellationToken token = default)
    {
        await packet.HandleAsync(this, token);
    }

    public Task HandleDisconnectAsync(IPacket packet, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public async Task HandleHandshakeAsync(IHandshakePacket packet, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug(
            "Received Handshake Packet from {RemoteEndPoint}: ProtocolVersion={ProtocolVersion}, ServerAddress={ServerAddress}, ServerPort={ServerPort}, NextState={NextState}",
            _clientConnection?.RemoteEndPoint?.ToString(), packet.ProtocolVersion, packet.ServerAddress,
            packet.ServerPort, packet.NextState);
        _clientConnection.ConnectionState = (ConnectionState)packet.NextState;

        await Task.CompletedTask;
    }

    public async Task HandleStatusRequestAsync(IStatusRequestPacket packet, CancellationToken token = default)
    {
        _logger.LogDebug("Received Status Request from {RemoteEndPoint}",
            _clientConnection?.RemoteEndPoint?.ToString());

        var serverStatus = new Status()
        {
            version = new Version()
            {
                Name = _serverOptions.Value.Instance.VersionName,
                Protocol = _serverOptions.Value.Instance.ProtocolVersion,
            },
            Players = new Players()
            {
                Max = _serverOptions.Value.MaxPlayers,
                Online = 0,
                Sample = [],
            },
            Description = new Description()
            {
                Text = _serverOptions.Value.Description,
            },
            EnforcesSecureChat = _serverOptions.Value.EnforcesSecureChat,
            Favicon = Utils.ConvertImageToBase64(_serverOptions.Value.FaviconPath),
        };

        var statusResponse = new StatusResponsePacket()
        {
            JsonResponse = JsonSerializer.Serialize(serverStatus, _jsonSerializerOptions)
        };

        if (_clientConnection != null)
        {
            await _clientConnection.SendPacketAsync(statusResponse);
        }
        else
        {
            _logger.LogError("Client connection is null in HandleStatusRequest");
        }
    }

    public async Task HandlePingPacketAsync(IPingPacket packet, CancellationToken token = default)
    {
        _logger.LogDebug("Received Ping Packet from {RemoteEndPoint}", _clientConnection?.RemoteEndPoint?.ToString());

        var pongPacket = new PongPacket()
        {
            Payload = packet.Payload
        };

        if (_clientConnection != null)
        {
            await _clientConnection.SendPacketAsync(pongPacket);
        }
        else
        {
            _logger.LogError("Client connection is null in HandlePingPacket");
        }
    }

    public Task HandleLoginPluginRequestAsync(ILoginPluginRequestPacket packet, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task HandleLoginAcknowledgedAsync(IPacket packet, CancellationToken cancellationToken)
    {
        _clientConnection.ConnectionState = ConnectionState.Configuration;
        return Task.CompletedTask;
    }

    public Task HandleCookieResponseAsync(IPacket packet, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public async Task HandleLoginStartAsync(ILoginStartPacket packet, CancellationToken token = default)
    {
        _logger.LogInformation("Player {PlayerName} is attempting to log in from {RemoteEndPoint}", packet.Username,
            _clientConnection?.RemoteEndPoint?.ToString());

        if (_serverOptions.Value.OnlineMode)
        {
            await HandleOnlineLogin();
        }
        else
        {
            await HandleOfflineLogin(packet.Username);

            if (_serverOptions.Value.Compression)
            {
                await HandleCompression();
            }
        }
    }

    private async Task HandleOnlineLogin()
    {
        var encryptionPacket = new EncryptionRequestPacket()
        {
            ServerId = string.Empty,
            PublicKey = _encryptionService.PublicKey,
            VerifyToken = _encryptionService.VerifyToken,
            ShouldAuthenticate = true,
        };

        await _clientConnection.SendPacketAsync(encryptionPacket);
    }


    private async Task HandleOfflineLogin(string username)
    {
        var loginSuccessPacket = new LoginSuccessPacket
        {
            MinecraftAccount = new MinecraftAccount
            {
                Id = new Guid().NewMinecraftGuid(username),
                Name = username,
                Properties = new List<Property>()
            }
        };
        await _clientConnection.SendPacketAsync(loginSuccessPacket);
    }


    private async Task HandleCompression()
    {
        var compressionPacket = new CompressionSetPacket()
        {
            Threshold = _serverOptions.Value.CompressionThreshold
        };
        await _clientConnection.SendPacketAsync(compressionPacket);
    }

    public async Task HandleEncryptionRequestAsync(IEncryptionResponsePacket packet,
        CancellationToken cancellationToken)
    {
        var sharedSecret = _encryptionService.Rsa.Decrypt(packet.EncryptedSharedSecret, RSAEncryptionPadding.Pkcs1);
        var decryptedVerifyToken =
            _encryptionService.Rsa.Decrypt(packet.EncryptedVerifyToken, RSAEncryptionPadding.Pkcs1);

        if (!decryptedVerifyToken.SequenceEqual(_encryptionService.VerifyToken))
        {
            _logger.LogError("decrypted verification token does not match shared secret!");
        }

        await EnableEncryption(sharedSecret);
    }

    private async Task EnableEncryption(byte[] sharedSecret)
    {
        //Validate Minecraft Account
        var minecraftAccount = await _encryptionService.ValidateMinecraftAccount(sharedSecret, "Cliplicht");
        if (minecraftAccount != null)
        {
            if (_serverOptions.Value.Compression)
            {
                await HandleCompression();
            }

            var loginSuccessPacket = new LoginSuccessPacket
            {
                MinecraftAccount = new MinecraftAccount
                {
                    Id = minecraftAccount.Id,
                    Name = minecraftAccount.Name,
                    Properties = minecraftAccount.Properties,
                }
            };
            await _clientConnection.SendPacketAsync(loginSuccessPacket);
            _clientConnection.ConnectionState = ConnectionState.Play;
        }
        else
        {
            _logger.LogError("Cannot validate minecraft account for {Username}", "");
        }
    }
}