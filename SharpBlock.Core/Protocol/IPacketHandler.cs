using SharpBlock.Core.Network;

namespace SharpBlock.Core.Protocol;

public interface IPacketHandler
{
    Task SetClientConnectionAsync(IClientConnection clientConnection, CancellationToken cancellationToken);
    Task HandlePacketAsync(IPacket packet, CancellationToken cancellationToken);
    Task HandleDisconnectAsync(IPacket packet, CancellationToken cancellationToken);
    Task HandleHandshakeAsync(IHandshakePacket packet, CancellationToken cancellationToken);
    Task HandleLoginStartAsync(ILoginStartPacket packet, CancellationToken cancellationToken);
    Task HandleStatusRequestAsync(IStatusRequestPacket packet, CancellationToken cancellationToken);
    Task HandlePingPacketAsync(IPingPacket packet, CancellationToken cancellationToken);
    Task HandleEncryptionRequestAsync(IEncryptionResponsePacket packet, CancellationToken cancellationToken);
    Task HandleLoginPluginRequestAsync(ILoginPluginRequestPacket packet, CancellationToken cancellationToken);
    Task HandleLoginAcknowledgedAsync(IPacket packet, CancellationToken cancellationToken);
    Task HandleCookieResponseAsync(IPacket packet, CancellationToken cancellationToken);
    Task HandleKeepAliveAsync(IKeepAlivePacket packet, CancellationToken cancellationToken);
    Task HandleClientSettingsAsync(IClientSettingsPacket packet, CancellationToken cancellationToken);
    Task HandlePluginMessageAsync(IPluginMessagePacket packet, CancellationToken cancellationToken);
    Task HandleKnownPacksAsync(IKnownPacksResponse packet, CancellationToken cancellationToken);
    Task HandleAcknowledgeFinishConfigurationAsync(IPacket packet, CancellationToken cancellationToken);
}