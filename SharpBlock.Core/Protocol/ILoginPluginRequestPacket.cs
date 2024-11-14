namespace SharpBlock.Core.Protocol;

public interface ILoginPluginRequestPacket : IPacket
{
    public string PluginName { get; set; }
}