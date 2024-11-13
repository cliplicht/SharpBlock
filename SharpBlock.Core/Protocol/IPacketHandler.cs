using SharpBlock.Core.Network;

namespace SharpBlock.Core.Protocol;

public interface IPacketHandler
{
    Task SetClientConnectionAsync(IClientConnection clientConnection);
    Task HandlePacketAsync(IPacket packet);
    Task HandleDisconnectAsync();
    Task HandleHandshakeAsync(IHandshakePacket packet);
    Task HandleLoginStartAsync(ILoginStartPacket packet);
    Task HandleStatusRequestAsync(IStatusRequestPacket packet);
    Task HandlePingPacketAsync(IPingPacket packet);
}