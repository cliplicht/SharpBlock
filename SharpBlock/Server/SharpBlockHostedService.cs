using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharpBlock.Options;

namespace SharpBlock.Server
{
    public class SharpBlockHostedService : IHostedService
    {
        private readonly ILogger<SharpBlockHostedService> _logger;
        private readonly EncryptionHandler _encryptionHandler;
        private readonly ServerOptions _serverOptions;
        private readonly WorldsOptions _worldOptions;

        private Socket _listenerSocket = null!;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public SharpBlockHostedService(
            ILogger<SharpBlockHostedService> logger,
            IOptions<ServerOptions> serverOptions,
            IOptions<WorldsOptions> worldOptions,
            EncryptionHandler encryptionHandler)
        {
            _logger = logger;
            _encryptionHandler = encryptionHandler;
            _serverOptions = serverOptions.Value;
            _worldOptions = worldOptions.Value;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _listenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _listenerSocket.Bind(new IPEndPoint(IPAddress.Any, _serverOptions.Port));
            _listenerSocket.Listen(1000);

            _logger.LogInformation("SharpBlock Server starting...");
            _logger.LogInformation($"Server is running on {_listenerSocket.LocalEndPoint}");

            // Start accepting connections asynchronously
            Task.Run(() => AcceptConnectionsAsync(_cancellationTokenSource.Token));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _cancellationTokenSource.Cancel();
            _listenerSocket.Close();
            return Task.CompletedTask;
        }

        private async Task AcceptConnectionsAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var clientSocket = await _listenerSocket.AcceptAsync();
                    _logger.LogInformation($"New connection from {clientSocket.RemoteEndPoint}");

                    // Create a new ClientConnection
                    var clientConnection = new ClientConnection(
                        _logger,
                        _serverOptions,
                        _worldOptions,
                        clientSocket,
                        _encryptionHandler);

                    // Start handling the client connection
                    clientConnection.StartHandling();
                }
                catch (SocketException ex)
                {
                    _logger.LogError($"SocketException in AcceptConnectionsAsync: {ex.Message}");
                }
                catch (ObjectDisposedException)
                {
                    // Listener socket has been closed, exit the loop
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Exception in AcceptConnectionsAsync: {ex.Message}");
                }
            }
        }
    }
}