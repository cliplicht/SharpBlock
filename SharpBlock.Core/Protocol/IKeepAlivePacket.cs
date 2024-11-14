namespace SharpBlock.Core.Protocol;

public interface IKeepAlivePacket : IPacket
{
    public long KeepAliveId { get; set; }
}