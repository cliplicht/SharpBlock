using Microsoft.Extensions.Logging;
using SharpBlock.Core;
using SharpBlock.Core.Protocol;
using SharpBlock.Protocol.Packets.Handshaking;
using SharpBlock.Protocol.Packets.Login;
using SharpBlock.Protocol.Packets.Ping;
using SharpBlock.Protocol.Packets.Status;

namespace SharpBlock.Protocol;

public class PacketFactory
{
    private readonly ILogger<PacketFactory> _logger;
    private readonly Dictionary<(ConnectionState, int), Func<Core.Protocol.IPacket>> _packetConstructors = new();

    public PacketFactory(ILogger<PacketFactory> logger)
    {
        _logger = logger;
        RegisterPackets();
    }

    private void RegisterPackets()
    {
        // Handshaking packets
        _packetConstructors[(ConnectionState.Handshaking, 0x00)] = () => new HandshakePacket();
        
        // Status packets
        _packetConstructors[(ConnectionState.Status, 0x00)] = () => new StatusRequestPacket();
        _packetConstructors[(ConnectionState.Status, 0x01)] = () => new PingPacket();

        // Login packets
        _packetConstructors[(ConnectionState.Login, 0x00)] = () => new LoginStartPacket();

        // Register other packets...
    }

    public IPacket CreatePacket(int packetId, ConnectionState state)
    {
        if (_packetConstructors.TryGetValue((state, packetId), out var constructor))
        {
            _logger.LogDebug("Packet {packetId} has created on state {state}", packetId, state);
            return constructor();
        }

        return null;
    }
}