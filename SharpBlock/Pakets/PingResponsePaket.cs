using SharpNBT.Extensions;

namespace SharpBlock.Pakets;

public class PingResponsePaket : OutgoingPaket
{
    public override int PacketId => 0x01;

    public long Payload { get; set; }

    public override void Write(Stream stream)
    {
        stream.WriteLong(Payload);
    }
}