using System.Threading.Channels;

namespace SharpBlock.Network.Events;

public class NetworkEventQueue
{
    private readonly Channel<NetworkEvent> _channel = Channel.CreateUnbounded<NetworkEvent>();

    public void Enqueue(NetworkEvent networkEvent)
    {
        _channel.Writer.TryWrite(networkEvent);
    }

    public async Task<NetworkEvent> DequeueAsync(CancellationToken cancellationToken)
    {
        return await _channel.Reader.ReadAsync(cancellationToken);
    }
}