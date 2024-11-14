using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharpBlock.Core;
using SharpBlock.Core.Network;
using SharpBlock.Core.Options;
using SharpBlock.Core.Protocol;
using SharpBlock.Core.Utils;
using SharpBlock.Network.Events;

namespace SharpBlock.Network.Core;

public class ClientConnection : IDisposable, IClientConnection
{
    private readonly TcpClient _tcpClient;
    private readonly NetworkStream _networkStream;
    private readonly ILogger<ClientConnection> _logger;
    private readonly ServerOptions _serverOptions;
    private readonly NetworkEventQueue _eventQueue;
    private readonly IServiceProvider _serviceProvider;

    // Buffer for incomplete data
    private byte[] _leftoverBuffer = Array.Empty<byte>();

    // Connection state
    public ConnectionState ConnectionState { get; set; } = ConnectionState.Handshaking;

    public ClientConnection(
        TcpClient tcpClient,
        ILogger<ClientConnection> logger,
        IOptions<ServerOptions> serverOptions,
        NetworkEventQueue eventQueue,
        IServiceProvider serviceProvider)
    {
        _tcpClient = tcpClient;
        _networkStream = tcpClient.GetStream();
        _logger = logger;
        _serverOptions = serverOptions.Value;
        _eventQueue = eventQueue;
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            await ReadLoopAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in client connection: {ex.Message}");
        }
        finally
        {
            Dispose();
        }
    }

    private async Task ReadLoopAsync(CancellationToken cancellationToken)
    {
        byte[] buffer = new byte[8192];
        byte[] leftoverBuffer = GetBufferedData();
        int leftoverBytes = leftoverBuffer.Length;

        while (!cancellationToken.IsCancellationRequested)
        {
            int bytesRead = await _networkStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);

            if (bytesRead == 0)
            {
                _logger.LogInformation("Client disconnected");
                _eventQueue.Enqueue(new ClientDisconnectedEvent(this));
                break;
            }
            _logger.LogInformation($"Received {bytesRead} bytes from client after handshake.");
            
            // Copy the data to a new array of the appropriate size
            byte[] data = new byte[bytesRead];
            Array.Copy(buffer, data, bytesRead);

            // Enqueue the DataReceivedEvent with the raw data
            _eventQueue.Enqueue(new DataReceivedEvent(this, data));
        }
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
        _tcpClient.Dispose();
    }

    // Methods to handle leftover data
    public byte[] GetBufferedData()
    {
        return _leftoverBuffer;
    }

    public void SetBufferedData(byte[] buffer, int offset, int count)
    {
        if (buffer == null)
            throw new ArgumentNullException(nameof(buffer));
        if (offset < 0 || count < 0 || offset + count > buffer.Length)
            throw new ArgumentOutOfRangeException("Invalid offset and count relative to buffer length.");

        _leftoverBuffer = new byte[count];
        Buffer.BlockCopy(buffer, offset, _leftoverBuffer, 0, count);
    }
    
    public void SetBufferedData(byte[] buffer)
    {
        _leftoverBuffer = buffer;
    }

    // Implement the missing ClearBufferedData method
    public void ClearBufferedData()
    {
        _leftoverBuffer = [];
    }

    // Implement the missing RemoteEndPoint property
    public EndPoint? RemoteEndPoint => _tcpClient.Client.RemoteEndPoint;
}