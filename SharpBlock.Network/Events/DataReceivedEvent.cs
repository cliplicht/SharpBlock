using SharpBlock.Network.Core;

namespace SharpBlock.Network.Events;

public class DataReceivedEvent(ClientConnection client, byte[] data, TaskCompletionSource processingCompletion)
    : NetworkEvent(client, processingCompletion)
{
    public byte[] Data { get; } = data;
}