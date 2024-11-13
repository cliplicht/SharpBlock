namespace SharpBlock.Core.Protocol;

public interface IPingPacket : IPacket
{
    public long Payload { get; set; }
}