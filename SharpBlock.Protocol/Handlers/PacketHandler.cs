using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SharpBlock.Core;
using SharpBlock.Core.Models;
using SharpBlock.Core.Network;
using SharpBlock.Core.Protocol;
using SharpBlock.Protocol.Packets.Login;
using SharpBlock.Protocol.Packets.Ping;
using SharpBlock.Protocol.Packets.Status;
using Version = SharpBlock.Core.Models.Version;

namespace SharpBlock.Protocol.Handlers;

public class PacketHandler : IPacketHandler
{
    private readonly ILogger<PacketHandler> _logger;
    private IClientConnection? _clientConnection;
    private JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.KebabCaseLower };

    public PacketHandler(ILogger<PacketHandler> logger)
    {
        _logger = logger;
    }

    public Task SetClientConnectionAsync(IClientConnection clientConnection)
    {
        _clientConnection = clientConnection;
        return Task.CompletedTask;
    }

    public async Task HandlePacketAsync(IPacket packet)
    {
        await packet.HandleAsync(this);
    }

    public Task HandleDisconnectAsync()
    {
        return Task.CompletedTask;
    }

    public async Task HandleHandshakeAsync(IHandshakePacket packet)
    {
        _logger.LogInformation(
            $"Received Handshake Packet from {_clientConnection?.RemoteEndPoint}: ProtocolVersion={packet.ProtocolVersion}, ServerAddress={packet.ServerAddress}, ServerPort={packet.ServerPort}, NextState={packet.NextState}");
        _clientConnection.ConnectionState = (ConnectionState)packet.NextState;

        await Task.CompletedTask;
    }
    
    public async Task HandleStatusRequestAsync(IStatusRequestPacket packet)
    {
        _logger.LogInformation($"Received Status Request from {_clientConnection?.RemoteEndPoint}");

        var serverStatus = new Status()
        {
            version = new Version()
            {
                Name = "1.21.3",
                Protocol = 768,
            },
            Players = new Players()
            {
                Max = 20,
                Online = 0,
                Sample = [],
            },
            Description = new Description()
            {
                Text = "Welcome to SharpBlock!",
            },
            EnforcesSecureChat = false,
            Favicon = string.Empty,
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
            _logger.LogError("Client connection is null in HandleStatusRequest.");
        }
    }
    
    public async Task HandlePingPacketAsync(IPingPacket packet)
    {
        _logger.LogInformation($"Received Ping Packet from {_clientConnection?.RemoteEndPoint}");

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
            _logger.LogError("Client connection is null in HandlePingPacket.");
        }
    }

    public async Task HandleLoginStartAsync(ILoginStartPacket packet)
    {
        _logger.LogInformation(
            $"Player {packet.Username} is attempting to log in from {_clientConnection?.RemoteEndPoint}.");

        // Send LoginSuccessPacket
        var loginSuccessPacket = new LoginSuccessPacket
        {
            UUID = Guid.NewGuid(),
            Username = packet.Username
        };

        if (_clientConnection != null)
        {
            await _clientConnection.SendPacketAsync(loginSuccessPacket);
            _clientConnection.ConnectionState = ConnectionState.Play;
        }
        else
        {
            _logger.LogError("Client connection is null in HandleLoginStart.");
        }
    }

    // Implement other packet handlers...
}