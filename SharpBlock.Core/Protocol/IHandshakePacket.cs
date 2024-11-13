namespace SharpBlock.Core.Protocol;

public interface IHandshakePacket : IPacket
{
    int ProtocolVersion { get; set; }
    string ServerAddress { get; set; }
    ushort ServerPort { get; set; }
    int NextState { get; set; }
}