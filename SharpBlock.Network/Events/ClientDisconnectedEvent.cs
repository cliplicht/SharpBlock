using SharpBlock.Core.Protocol;
using SharpBlock.Network.Core;

namespace SharpBlock.Network.Events;

public class ClientDisconnectedEvent(ClientConnection client, IPacket packet, TaskCompletionSource processingCompletion)
    : NetworkEvent(client, processingCompletion)
{
    public IPacket Packet { get; } = packet;
}