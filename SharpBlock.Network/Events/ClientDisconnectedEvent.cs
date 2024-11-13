using SharpBlock.Network.Core;

namespace SharpBlock.Network.Events;

public class ClientDisconnectedEvent : NetworkEvent
{
    public ClientDisconnectedEvent(ClientConnection client)
        : base(client)
    {
    }
}