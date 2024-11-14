using Microsoft.Extensions.Logging;
using SharpBlock.Core;
using SharpBlock.Core.Protocol;
using SharpBlock.Protocol.Packets;
using SharpBlock.Protocol.Packets.Configuration;
using SharpBlock.Protocol.Packets.Cookie;
using SharpBlock.Protocol.Packets.Encryption;
using SharpBlock.Protocol.Packets.Handshaking;
using SharpBlock.Protocol.Packets.Login;
using SharpBlock.Protocol.Packets.Ping;
using SharpBlock.Protocol.Packets.Play;
using SharpBlock.Protocol.Packets.Plugin;
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
        _packetConstructors[(ConnectionState.Login, 0x01)] = () => new EncryptionResponsePacket();
        _packetConstructors[(ConnectionState.Login, 0x02)] = () => new LoginPluginResponsePacket();
        _packetConstructors[(ConnectionState.Login, 0x03)] = () => new LoginAcknowledgePacket();
        _packetConstructors[(ConnectionState.Login, 0x04)] = () => new CookieResponsePacket();

        //Configuration
        _packetConstructors[(ConnectionState.Configuration, 0x00)] = () => new ClientSettingsPacket();
        _packetConstructors[(ConnectionState.Configuration, 0x01)] = () => new CookieResponsePacket();
        _packetConstructors[(ConnectionState.Configuration, 0x02)] = () => new PluginMessagePacket();
        _packetConstructors[(ConnectionState.Configuration, 0x03)] = () => new AcknowledgeFinishConfigurationPacket();
        _packetConstructors[(ConnectionState.Configuration, 0x04)] = () => new KeepAlivePacket();
        _packetConstructors[(ConnectionState.Configuration, 0x05)] = () => new PingPacket();
        _packetConstructors[(ConnectionState.Configuration, 0x06)] = () => new ResourcePackResponse();
        _packetConstructors[(ConnectionState.Configuration, 0x07)] = () => new KnownPacksResponse();
        
        
        // Play packets
        _packetConstructors[(ConnectionState.Play, 0x00)] = () => new KeepAlivePacket();
        _packetConstructors[(ConnectionState.Play, 0x02)] = () => new ClientSettingsPacket();
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