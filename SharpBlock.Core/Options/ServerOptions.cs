namespace SharpBlock.Core.Options
{
    public class ServerOptions
    {
        public const string Server = "Server";
        public InstanceOptions Instance { get; set; } = new();
        public bool OnlineMode { get; set; } = true;
        public bool Compression { get; set; } = true;
        public string FaviconPath { get; set; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"serverFaviconSmall.png");
        public bool EnforcesSecureChat { get; set; } = false;
        public int CompressionThreshold { get; set; } = 256;
        public int Port { get; set; } = 25565;
        public int MaxPlayers { get; set; } = 20;
        public string Description { get; set; } = "Welcome to SharpBlock!";
        public bool IsHardcore { get; set; } = false;
        public byte GameMode { get; set; } = 1;
        public int ViewDistance { get; set; } = 10;
        public int SimulationDistance { get; set; } = 10;
    }
}