using SharpBlock.Network.Core;

namespace SharpBlock.Network.Events;

public class DataReceivedEvent : NetworkEvent
{
    public byte[] Data { get; }

    public DataReceivedEvent(ClientConnection client, byte[] data)
        : base(client)
    {
        Data = data;
    }
}