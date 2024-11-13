using SharpNBT.Extensions;

namespace SharpBlock.Pakets;

public class SetCompressionPacket : OutgoingPaket
{
    public override int PacketId { get; } = 0x03;
    public int Threshold { get; set; }
    public override void Write(Stream stream)
    {
       stream.WriteVarInt(Threshold);
    }
}