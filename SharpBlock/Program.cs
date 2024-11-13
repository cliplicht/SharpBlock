using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SharpBlock.Config;
using SharpBlock.Options;
using SharpBlock.Server;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Services.AddOptions();

// Setup Configuration
IConfigurationRoot config = new ConfigurationBuilder()
    .AddJsonFile("serverOptions.json", optional: true, reloadOnChange: true)
    .AddJsonFile("worldsOptions.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

builder.Configuration.AddConfiguration(config);

// Options
builder.Services.Configure<WorldsOptions>(builder.Configuration.GetSection("Worlds"));
builder.Services.Configure<ServerOptions>(builder.Configuration.GetSection("Server"));


var serverOptions = new ServerOptions();
builder.Configuration.GetSection("Server").Bind(serverOptions);


// Logging
builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.AddConsole();
    loggingBuilder.SetMinimumLevel(serverOptions.Instance.LogLevel);
});


builder.Services.AddSingleton<EncryptionHandler>();
builder.Services.AddHostedService<SharpBlockHostedService>();


using IHost host = builder.Build();

await host.RunAsync();