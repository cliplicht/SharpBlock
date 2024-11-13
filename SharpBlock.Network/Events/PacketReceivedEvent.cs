using SharpBlock.Core.Network;
using SharpBlock.Core.Protocol;

namespace SharpBlock.Network.Events;

public class PacketReceivedEvent : NetworkEvent
{
    public IPacket Packet { get; }

    public PacketReceivedEvent(IClientConnection client, IPacket packet)
        : base(client)
    {
        Packet = packet;
    }
}