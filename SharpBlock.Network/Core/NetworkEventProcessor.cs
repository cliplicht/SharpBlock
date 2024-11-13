using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SharpBlock.Network.Events;
using SharpBlock.Protocol;
using SharpBlock.Protocol.Handlers;

namespace SharpBlock.Network.Core;

public class NetworkEventProcessor
{
    private readonly NetworkEventQueue _eventQueue;
    private readonly ILogger<NetworkEventProcessor> _logger;
    private readonly IServiceProvider _serviceProvider;

    public NetworkEventProcessor(
        NetworkEventQueue eventQueue,
        ILogger<NetworkEventProcessor> logger,
        IServiceProvider serviceProvider)
    {
        _eventQueue = eventQueue;
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public async Task StartProcessingAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("NetworkEventProcessor started.");

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                NetworkEvent networkEvent = await _eventQueue.DequeueAsync(cancellationToken);

                await HandleEventAsync(networkEvent, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                // The operation was canceled; exit the loop gracefully.
                _logger.LogInformation("NetworkEventProcessor is stopping due to cancellation.");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing network event.");
            }
        }

        _logger.LogInformation("NetworkEventProcessor has stopped.");
    }

    private async Task HandleEventAsync(NetworkEvent networkEvent, CancellationToken cancellationToken)
    {
        switch (networkEvent)
        {
            case DataReceivedEvent dataEvent:
                await HandleDataReceivedAsync(dataEvent, cancellationToken);
                break;

            case PacketReceivedEvent packetEvent:
                await HandlePacketReceivedAsync(packetEvent, cancellationToken);
                break;

            case ClientDisconnectedEvent disconnectedEvent:
                HandleClientDisconnected(disconnectedEvent);
                break;

            default:
                _logger.LogWarning($"Unhandled network event type: {networkEvent.GetType().Name}");
                break;
        }
    }

    private async Task HandleDataReceivedAsync(DataReceivedEvent dataEvent, CancellationToken cancellationToken)
    {
        var client = dataEvent.Client;
        var packetParser = _serviceProvider.GetRequiredService<PacketParser>();

        // Combine leftover data with new data
        byte[] bufferedData = client.GetBufferedData();
        byte[] combinedData = CombineBuffers(bufferedData, dataEvent.Data);

        int leftoverBytes = 0;
        var packets = packetParser.Parse(combinedData, combinedData.Length, client.ConnectionState, ref leftoverBytes);

        foreach (var packet in packets)
        {
            // Enqueue PacketReceivedEvent for each parsed packet
            _eventQueue.Enqueue(new PacketReceivedEvent(client, packet));
        }

        // Store leftover bytes in client for next read
        if (leftoverBytes > 0)
        {
            client.SetBufferedData(combinedData, combinedData.Length - leftoverBytes, leftoverBytes);
        }
        else
        {
            client.ClearBufferedData();
        }
    }

    private async Task HandlePacketReceivedAsync(PacketReceivedEvent packetEvent, CancellationToken cancellationToken)
    {
        var packetHandler = ActivatorUtilities.CreateInstance<PacketHandler>(_serviceProvider);
        await packetHandler.SetClientConnectionAsync(packetEvent.Client);
        await packetHandler.HandlePacketAsync(packetEvent.Packet);
    }

    private async Task HandleClientDisconnected(ClientDisconnectedEvent disconnectedEvent)
    {
        _logger.LogInformation($"Client disconnected: {disconnectedEvent.Client.RemoteEndPoint}");
        var packetHandler = ActivatorUtilities.CreateInstance<PacketHandler>(_serviceProvider);
        await packetHandler.SetClientConnectionAsync(disconnectedEvent.Client);
        await packetHandler.HandleDisconnectAsync();
    }

    private byte[] CombineBuffers(byte[] buffer1, byte[] buffer2)
    {
        byte[] combined = new byte[buffer1.Length + buffer2.Length];
        Buffer.BlockCopy(buffer1, 0, combined, 0, buffer1.Length);
        Buffer.BlockCopy(buffer2, 0, combined, buffer1.Length, buffer2.Length);
        return combined;
    }
}