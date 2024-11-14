using SharpBlock.Core.Protocol;
using SharpBlock.Core.Utils;

namespace SharpBlock.Protocol.Packets.Compression;

public class CompressionSetPacket : ICompressionPacket
{
    public int PacketId { get; } = 0x03;
    public int Threshold { get; set; }
    public void Read(Stream stream)
    {
        throw new NotImplementedException();
    }

    public void Write(Stream stream)
    {
        stream.WriteVarInt(Threshold);
    }

    public Task HandleAsync(IPacketHandler handler, CancellationToken token)
    {
        throw new NotImplementedException();
    }

    
}