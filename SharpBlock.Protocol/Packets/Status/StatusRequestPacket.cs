using SharpBlock.Core.Protocol;

namespace SharpBlock.Protocol.Packets.Status;

public class StatusRequestPacket : IStatusRequestPacket
{
    public int PacketId { get; } = 0x00;
    public void Read(Stream stream)
    {
        
    }

    public void Write(Stream stream)
    {
        
    }

    public async Task HandleAsync(IPacketHandler handler, CancellationToken cancellationToken = default)
    {
        await handler.HandleStatusRequestAsync(this, cancellationToken);
    }
}