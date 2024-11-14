using SharpBlock.Core.Protocol;

namespace SharpBlock.Protocol.Packets.Configuration;

public class FinishConfigurationPacket : IFinishConfigurationPacket
{
    public int PacketId { get; } = 0x03;
    public void Read(Stream stream)
    {
        
    }

    public void Write(Stream stream)
    {
        
    }

    public async Task HandleAsync(IPacketHandler handler, CancellationToken token)
    {
    }
}