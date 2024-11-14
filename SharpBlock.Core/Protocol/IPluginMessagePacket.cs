namespace SharpBlock.Core.Protocol;

public interface IPluginMessagePacket : IPacket
{
    public string Channel { get; set; }
    public byte[] Data { get; set; }
}