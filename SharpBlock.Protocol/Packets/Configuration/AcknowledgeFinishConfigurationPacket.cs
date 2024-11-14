using SharpBlock.Core.Protocol;

namespace SharpBlock.Protocol.Packets.Configuration;

public class AcknowledgeFinishConfigurationPacket : IPacket
{
    public int PacketId { get; } = 0x04;
    public void Read(Stream stream)
    {
        
    }

    public void Write(Stream stream)
    {
        
    }

    public async Task HandleAsync(IPacketHandler handler, CancellationToken token)
    {
        await handler.HandleAcknowledgeFinishConfigurationAsync(this, token);
    }
}