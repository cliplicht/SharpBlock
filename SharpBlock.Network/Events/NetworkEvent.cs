using SharpBlock.Core.Network;

namespace SharpBlock.Network.Events;

public abstract class NetworkEvent
{
    public IClientConnection Client { get; }
    public TaskCompletionSource ProcessingCompletion { get; }

    protected NetworkEvent(IClientConnection client,  TaskCompletionSource processingCompletion)
    {
        Client = client;
        ProcessingCompletion = processingCompletion;
    }
}