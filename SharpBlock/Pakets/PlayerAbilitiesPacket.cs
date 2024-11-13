using SharpNBT.Extensions;

namespace SharpBlock.Pakets;

public class PlayerAbilitiesPacket : OutgoingPaket
{
    public override int PacketId => 0x32; // Packet ID für 1.19.4

    public byte Flags { get; set; }
    public float FlyingSpeed { get; set; }
    public float WalkingSpeed { get; set; }

    public override void Write(Stream stream)
    {
        stream.WriteUnsignedByte(Flags);
        stream.WriteFloat(FlyingSpeed);
        stream.WriteFloat(WalkingSpeed);
    }
}