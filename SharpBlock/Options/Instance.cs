using Microsoft.Extensions.Logging;

namespace SharpBlock.Config
{
    public class Instance
    {
        public LogLevel LogLevel { get; set; } = LogLevel.Information;
        public int ProtocolVersion { get; set; } = 768;
        public string VersionName { get; set; } = "1.21.3";
        public int MaxConnections { get; set; } = 10000;
        public int BufferSize { get; set; } = 1024;
    }
}