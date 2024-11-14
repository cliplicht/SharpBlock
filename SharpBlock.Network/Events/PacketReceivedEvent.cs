using SharpBlock.Core.Network;
using SharpBlock.Core.Protocol;

namespace SharpBlock.Network.Events;

public class PacketReceivedEvent(IClientConnection client, IPacket packet, TaskCompletionSource processingCompletion)
    : NetworkEvent(client, processingCompletion)
{
    public IPacket Packet { get; } = packet;
}