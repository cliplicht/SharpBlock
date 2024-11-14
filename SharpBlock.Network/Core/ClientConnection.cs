using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharpBlock.Core;
using SharpBlock.Core.Network;
using SharpBlock.Core.Options;
using SharpBlock.Core.Protocol;
using SharpBlock.Core.Utils;
using SharpBlock.Network.Events;
using SharpBlock.Protocol;

namespace SharpBlock.Network.Core;

public class ClientConnection : IDisposable, IClientConnection
{
    public readonly TcpClient TcpClient;
    private readonly NetworkStream _networkStream;
    private readonly ILogger<ClientConnection> _logger;
    private readonly ServerOptions _serverOptions;
    private readonly NetworkEventQueue _eventQueue;
    private readonly IServiceProvider _serviceProvider;
    private readonly PacketFactory _packetFactory;
    private readonly SemaphoreSlim _packetProcessingLock = new(1, 1);

    // Connection state
    public ConnectionState ConnectionState { get; set; } = ConnectionState.Handshaking;

    public ClientConnection(
        TcpClient tcpClient,
        ILogger<ClientConnection> logger,
        IOptions<ServerOptions> serverOptions,
        NetworkEventQueue eventQueue,
        IServiceProvider serviceProvider)
    {
        TcpClient = tcpClient;
        _networkStream = tcpClient.GetStream();
        _logger = logger;
        _serverOptions = serverOptions.Value;
        _eventQueue = eventQueue;
        _serviceProvider = serviceProvider;
        _packetFactory = _serviceProvider.GetRequiredService<PacketFactory>();
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            await ReceivePacketsAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error in client connection: {ExceptionMessage}", ex.Message);
        }
        finally
        {
            Dispose();
        }
    }

    private async Task ReceivePacketsAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            await _packetProcessingLock.WaitAsync(cancellationToken);
            var processingCompletion = new TaskCompletionSource();
            try
            {
                IPacket packet = await ReadPacketAsync(cancellationToken);
                _eventQueue.Enqueue(new PacketReceivedEvent(this, packet, processingCompletion));
            }
            catch (IOException)
            {
                IPacket packet = await ReadPacketAsync(cancellationToken);
                _eventQueue.Enqueue(new ClientDisconnectedEvent(this, packet ,processingCompletion));
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error receiving data");
                IPacket packet = await ReadPacketAsync(cancellationToken);
                _eventQueue.Enqueue(new ClientDisconnectedEvent(this, packet,processingCompletion));
                break;
            }
            finally
            {
                await processingCompletion.Task;
                _packetProcessingLock.Release();
            }
        }
    }

    public async Task<IPacket> ReadPacketAsync(CancellationToken cancellationToken)
    {
        // Read the VarInt packet length
        int packetLength = await _networkStream.ReadVarIntAsync(cancellationToken);

        if (packetLength <= 0)
        {
            _logger.LogWarning("Invalid packet length: {PacketLength}",packetLength);
            throw new IOException("Invalid packet length.");
        }

        // Read the packet data based on the packet length
        byte[] packetData = new byte[packetLength];
        int totalBytesRead = 0;
        while (totalBytesRead < packetLength)
        {
            int bytesRead = await _networkStream.ReadAsync(packetData, totalBytesRead, packetLength - totalBytesRead, cancellationToken);
            if (bytesRead == 0)
            {
                // The client has disconnected
                throw new IOException("Client disconnected");
            }
            totalBytesRead += bytesRead;
        }

        using var ms = new MemoryStream(packetData);
        int packetId = await ms.ReadVarIntAsync(cancellationToken);

        // Create packet based on the current connection state
        IPacket packet = _packetFactory.CreatePacket(packetId, ConnectionState);

        if (packet == null)
        {
            _logger.LogDebug("Unknown packetID [{PacketId}] | State: [{State}]", packetId, ConnectionState);
            return null;
        }
        else
        {
            packet.Read(ms);
        }
        return packet;
    }

    public async Task SendPacketAsync(IPacket packet)
    {
        using var ms = new MemoryStream();
        ms.WriteVarInt(packet.PacketId);
        packet.Write(ms);
        byte[] packetData = ms.ToArray();

        using var finalStream = new MemoryStream();
        finalStream.WriteVarInt(packetData.Length);
        finalStream.Write(packetData, 0, packetData.Length);

        byte[] finalPacket = finalStream.ToArray();
        await _networkStream.WriteAsync(finalPacket, 0, finalPacket.Length);
        await _networkStream.FlushAsync();
    }

    public void Dispose()
    {
        _networkStream.Dispose();
        TcpClient.Dispose();
    }

    // Implement the RemoteEndPoint property
    public EndPoint? RemoteEndPoint
    {
        get
        {
            try
            {
                if (TcpClient?.Client is { Connected: true })
                {
                    return TcpClient.Client.RemoteEndPoint;
                }
                else
                {
                    _logger.LogDebug("Attempted to access RemoteEndPoint on a disconnected or disposed client");
                    return null;
                }
            }
            catch (ObjectDisposedException)
            {
                _logger.LogDebug("Attempted to access RemoteEndPoint on a disposed client");
                return null;
            }
        }
    }
}