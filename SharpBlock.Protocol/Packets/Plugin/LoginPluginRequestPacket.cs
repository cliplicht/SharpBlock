using SharpBlock.Core.Protocol;
using SharpBlock.Core.Utils;

namespace SharpBlock.Protocol.Packets;

public class LoginPluginRequestPacket : ILoginPluginRequestPacket
{
    public int PacketId { get; } = 0x02;
    public string PluginName { get; set; } = string.Empty;

    public void Read(Stream stream)
    {
        PluginName = stream.ReadString();
    }

    public void Write(Stream stream)
    {
        stream.WriteString(PluginName);
    }

    public async Task HandleAsync(IPacketHandler handler, CancellationToken token)
    {
        await handler.HandleLoginPluginRequestAsync(this, token);
    }

    
}