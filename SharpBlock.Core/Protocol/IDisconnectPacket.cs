namespace SharpBlock.Core.Protocol;

public interface IDisconnectPacket : IPacket
{
    public string Reason { get; set; }
}