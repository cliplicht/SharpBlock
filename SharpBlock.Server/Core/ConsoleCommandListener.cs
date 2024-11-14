using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SharpBlock.Server.Core;

public class ConsoleCommandListener(ILogger<ConsoleCommandListener> logger, IHostApplicationLifetime appLifetime)
    : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogDebug("Command listener started");

        Task.Run(() =>
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                
                if (!Console.KeyAvailable)
                {
                    Thread.Sleep(100);
                    continue;
                }
                
                string? input = Console.ReadLine();
                if (string.IsNullOrEmpty(input)) continue;

                ClearCurrentConsoleLine();

                switch (input.ToLower())
                {
                    case "stop":
                        logger.LogInformation("Shutting down the server...");
                        appLifetime.StopApplication();
                        break;
                    case "clear":
                        Console.Clear();
                        break;
                    default:
                        logger.LogWarning("Unknown command: {Input}", input);
                        break;
                }
            }
        }, stoppingToken);

        return Task.CompletedTask;
    }
    
    private void ClearCurrentConsoleLine()
    {
        int currentLineCursor = Console.CursorTop;
        Console.SetCursorPosition(0, currentLineCursor - 1);
        Console.Write(new string(' ', Console.WindowWidth));
        Console.SetCursorPosition(0, currentLineCursor - 1);
    }
}