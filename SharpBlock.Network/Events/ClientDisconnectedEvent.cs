using SharpBlock.Network.Core;

namespace SharpBlock.Network.Events;

public class ClientDisconnectedEvent(ClientConnection client, TaskCompletionSource processingCompletion)
    : NetworkEvent(client, processingCompletion);