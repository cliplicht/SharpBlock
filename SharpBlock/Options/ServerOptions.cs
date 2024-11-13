using SharpBlock.Config;

namespace SharpBlock.Options
{
    public class ServerOptions
    {
        public const string Server = "Server";
        public Instance Instance { get; set; } = null!;
        public bool OnlineMode { get; set; } = true;
        public bool Encryption { get; set; } = true;
        public bool Compression { get; set; } = false;
        public int CompressionThreshold { get; set; } = 256;
        public int Port { get; set; }
        public int MaxPlayers { get; set; }
        public string Description { get; set; } = null!;
        public bool IsHardcore { get; set; }
        public byte GameMode { get; set; }
        public int ViewDistance { get; set; }
        public int SimulationDistance { get; set; }
    }
}