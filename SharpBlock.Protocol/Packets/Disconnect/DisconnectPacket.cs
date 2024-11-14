using SharpBlock.Core.Protocol;
using SharpBlock.Core.Utils;

namespace SharpBlock.Protocol.Packets.Disconnect;

public class DisconnectPacket : IDisconnectPacket
{
    public int PacketId { get; } = 0x00;
    public string Reason { get; set; } = string.Empty;
    
    public void Read(Stream stream)
    {
        Reason = stream.ReadString();
    }

    public void Write(Stream stream)
    {
        stream.WriteString(Reason);
    }

    public async Task HandleAsync(IPacketHandler handler, CancellationToken token = default)
    {
        await handler.HandleDisconnectAsync(this, token);
    }
}