using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharpBlock.Core.Options;
using SharpBlock.Network.Core;

namespace SharpBlock.Server.Core;

public class SharpBlockHostedService : IHostedService
{
    private readonly ILogger<SharpBlockHostedService> _logger;
    private readonly IOptions<ServerOptions> _serverOptions;
    private readonly IServiceProvider _serviceProvider;
    private NetworkEventProcessor? _eventProcessor;

    private TcpListener? _tcpListener;
    private CancellationTokenSource? _cts;

    public SharpBlockHostedService(
        ILogger<SharpBlockHostedService> logger,
        IOptions<ServerOptions> serverOptions,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serverOptions = serverOptions;
        _serviceProvider = serviceProvider;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting SharpBlock Server...");

        int port = _serverOptions.Value.Port;
        _tcpListener = new TcpListener(IPAddress.Any, port);
        _tcpListener.Start();

        _logger.LogInformation("Server started {EndPointAddress}", _tcpListener.LocalEndpoint.ToString());

        _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        // Start accepting clients asynchronously
        Task.Run(() => AcceptClientsAsync(_cts.Token), _cts.Token);

        // Start the NetworkEventProcessor
        _eventProcessor = ActivatorUtilities.CreateInstance<NetworkEventProcessor>(_serviceProvider);
        Task.Run(() => _eventProcessor.StartProcessingAsync(_cts.Token), _cts.Token);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping SharpBlock Server...");

        _cts?.Cancel();
        _tcpListener?.Stop();

        return Task.CompletedTask;
    }

    private async Task AcceptClientsAsync(CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                TcpClient tcpClient = await _tcpListener!.AcceptTcpClientAsync(cancellationToken);

                _logger.LogDebug("New connection from {RemoteEndPoint}",
                    tcpClient.Client.RemoteEndPoint?.ToString());

                // Handle client connection
                _ = Task.Run(() => HandleClientAsync(tcpClient, cancellationToken), cancellationToken);
            }
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError("Exception in AcceptClientsAsync: {ExceptionMessage}", ex.Message);
        }
    }

    private async Task HandleClientAsync(TcpClient tcpClient, CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var clientConnection = ActivatorUtilities.CreateInstance<ClientConnection>(
            scope.ServiceProvider, tcpClient);

        try
        {
            await clientConnection.StartAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError("Exception handling client {RemoteEndPoint}: {ExceptionMessage}",
                tcpClient.Client.RemoteEndPoint?.ToString(), ex.Message);
        }
    }
}