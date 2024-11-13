namespace SharpBlock.Core.Protocol;

public interface IPongPacket : IPacket
{
    public long Payload { get; set; }
}