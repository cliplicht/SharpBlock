namespace SharpBlock.Core.Protocol;

public interface ICompressionPacket : IPacket
{
    public int Threshold { get; set; }
}