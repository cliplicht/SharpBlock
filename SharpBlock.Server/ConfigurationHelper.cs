using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace SharpBlock.Server;

public static class ConfigurationHelper
{
    public static IConfiguration GetConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("server.json");
        
        IConfigurationRoot configuration = builder.Build();

        if (!File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "server.json")))
        {
            CreateDefaultConfiguration();
        }
        
        return configuration;
    }

    private static void CreateDefaultConfiguration()
    {
        var defaultConfig = new
        {
            Server = new
            {
                Port = 25565,
                OnlineMode = true,
                MaxPlayers = 100,
                COmpression = false,
                CompressionThreshold = 256,
                Encryption = true,
                Instance = new
                {
                    VersionName = "1.21.3",
                    ProtocolVersion = 768
                },
                Description = "Welcome to SharpBlock Server!",
            }
        };
    

    string json = JsonSerializer.Serialize(defaultConfig);
        File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "server.json"), json);
    }
}