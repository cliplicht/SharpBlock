using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SharpBlock.Core.Options;
using SharpBlock.Network.Core;
using SharpBlock.Network.Events;
using SharpBlock.Protocol;
using SharpBlock.Protocol.Handlers;
using SharpBlock.Server.Core;

namespace SharpBlock.Server;

static class Program
{
    public static async Task Main(string[] args)
    {
        await EnsureServerJsonExists();
        using IHost host = CreateHostBuilder(args).Build();
        await host.RunAsync();
    }


    private static async Task EnsureServerJsonExists()
    {
        string configFileName = "server.json";
        string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configFileName);
        
        if (!File.Exists(path))
        {
            var config = new ServerOptions()
            {
                Instance = new Instance()
            };
            string json = JsonSerializer.Serialize(config);
            await File.WriteAllTextAsync(path,json);
        }
    }
    

    static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
            })
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.SetBasePath(AppDomain.CurrentDomain.BaseDirectory);
                config.AddJsonFile("server.json", optional: true, reloadOnChange: true);
                config.AddEnvironmentVariables();
            })
            .ConfigureServices((hostContext, services) =>
            {
                IConfiguration configuration = hostContext.Configuration;
                services.Configure<ServerOptions>(configuration.GetSection("Server"));

                services.AddSingleton<NetworkEventQueue>();
                services.AddSingleton<PacketFactory>();
                services.AddSingleton<NetworkEventProcessor>();

                services.AddTransient<PacketParser>();
                services.AddTransient<PacketHandler>();

                services.AddHostedService<SharpBlockHostedService>();
            });
}