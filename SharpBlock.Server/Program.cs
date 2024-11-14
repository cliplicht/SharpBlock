using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharpBlock.Core.Options;
using SharpBlock.Core.Services;
using SharpBlock.Core.Utils;
using SharpBlock.Network.Core;
using SharpBlock.Network.Events;
using SharpBlock.Protocol;
using SharpBlock.Protocol.Handlers;
using SharpBlock.Server.Core;


await EnsureServerJsonExists();
var builder = Host.CreateDefaultBuilder(args);


builder.UseEnvironment(CustomEnvironments.Java);

builder.ConfigureAppConfiguration((hostingContext, config) =>
{
    config.SetBasePath(AppDomain.CurrentDomain.BaseDirectory);
    config.AddJsonFile("server.json", optional: false, reloadOnChange: true);
    config.AddEnvironmentVariables();
    config.AddEnvironmentVariables(prefix: "SHARPBLOCK_");
});


builder.ConfigureLogging((context,logging) =>
{
    logging.ClearProviders();
                
    logging.AddConsole(options =>
    {
        options.FormatterName = "fancy";
    });
    logging.AddConsoleFormatter<FancyLogFormatter, FancyLogFormatterOptions>();
    logging.SetMinimumLevel(context.Configuration.GetRequiredSection(ServerOptions.Server)
        .GetValue<LogLevel>("Instance:LogLevel"));
});

builder.ConfigureServices((context, services) =>
{
    services.Configure<ServerOptions>(context.Configuration.GetSection(ServerOptions.Server));
    services.Configure<InstanceOptions>(context.Configuration.GetSection(InstanceOptions.Instance));
    services.AddSingleton<NetworkEventQueue>();
    services.AddSingleton<PacketFactory>();
    services.AddSingleton<NetworkEventProcessor>();
    services.AddHttpClient();
    services.AddSingleton<EncryptionService>();
    services.AddTransient<PacketHandler>();

    services.AddHostedService<SharpBlockHostedService>();
    services.AddHostedService<ConsoleCommandListener>();
});


var app = builder.Build();


await app.RunAsync();

async Task EnsureServerJsonExists()
{
    string configFileName = "server.json";
    string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configFileName);
        
    if (!File.Exists(path))
    {
        var jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            Converters =
            {
                new JsonStringEnumConverter()
            }
        };
            
        string json = JsonSerializer.Serialize(new ServerOptions(), jsonOptions);
        await File.WriteAllTextAsync(path,json);
    }
}
