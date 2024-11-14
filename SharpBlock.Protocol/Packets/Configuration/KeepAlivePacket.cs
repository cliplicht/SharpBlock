using SharpBlock.Core.Protocol;
using SharpBlock.Core.Utils;

namespace SharpBlock.Protocol.Packets.Play;

public class KeepAlivePacket : IKeepAlivePacket
{
    public int PacketId { get; } = 0x00;
    public long KeepAliveId { get; set; }
    public void Read(Stream stream)
    {
        KeepAliveId = stream.ReadLong();
    }

    public void Write(Stream stream)
    {
        stream.WriteLong(KeepAliveId);
    }

    public async Task HandleAsync(IPacketHandler handler, CancellationToken token)
    {
        await handler.HandleKeepAliveAsync(this, token);
    }

    
}