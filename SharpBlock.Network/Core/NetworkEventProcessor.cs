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
    private readonly SemaphoreSlim _eventProcessingLock = new(1, 1);

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
        _logger.LogDebug("NetworkEventProcessor started");

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
                _logger.LogDebug("NetworkEventProcessor is stopping due to cancellation");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing network event");
            }
        }

        _logger.LogDebug("NetworkEventProcessor has stopped");
    }

    private async Task HandleEventAsync(NetworkEvent networkEvent, CancellationToken cancellationToken)
    {
        if (networkEvent.Client is ClientConnection clientConnection && !clientConnection.TcpClient.Connected)
        {
            _logger.LogDebug("Skipping event processing for a disconnected client");
            return;
        }

        await _eventProcessingLock.WaitAsync(cancellationToken);
        try
        {
            switch (networkEvent)
            {
                case PacketReceivedEvent packetEvent:
                    await HandlePacketReceivedAsync(packetEvent, cancellationToken);
                    break;

                case ClientDisconnectedEvent disconnectedEvent:
                    await HandleClientDisconnected(disconnectedEvent, cancellationToken);
                    break;

                default:
                    _logger.LogDebug("Unhandled network event type: {NetworkEventType}", networkEvent.GetType().Name);
                    break;
            }
        }
        finally
        {
            _eventProcessingLock.Release();
        }
    }


    private async Task HandlePacketReceivedAsync(PacketReceivedEvent packetEvent, CancellationToken cancellationToken)
    {
        var packetHandler = ActivatorUtilities.CreateInstance<PacketHandler>(_serviceProvider);
        await packetHandler.SetClientConnectionAsync(packetEvent.Client, cancellationToken);
        await packetHandler.HandlePacketAsync(packetEvent.Packet, cancellationToken);

        packetEvent.ProcessingCompletion.SetResult();
    }

    private async Task HandleClientDisconnected(ClientDisconnectedEvent disconnectedEvent, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Client disconnected: {RemoteEndPoint}",
            disconnectedEvent.Client.RemoteEndPoint?.ToString());
        var packetHandler = ActivatorUtilities.CreateInstance<PacketHandler>(_serviceProvider);
        await packetHandler.SetClientConnectionAsync(disconnectedEvent.Client, cancellationToken);
        await packetHandler.HandleDisconnectAsync(disconnectedEvent.Packet, cancellationToken);
        disconnectedEvent.ProcessingCompletion.SetResult();
    }
}