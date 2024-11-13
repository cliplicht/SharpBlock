using SharpBlock.Core.Network;

namespace SharpBlock.Network.Events;

public abstract class NetworkEvent
{
    public IClientConnection Client { get; }

    protected NetworkEvent(IClientConnection client)
    {
        Client = client;
    }
}