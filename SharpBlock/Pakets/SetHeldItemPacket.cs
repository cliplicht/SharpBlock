using SharpNBT.Extensions;

namespace SharpBlock.Pakets;

public class SetHeldItemPacket : OutgoingPaket
{
    public override int PacketId => 0x47; // Packet ID für 1.19.4

    public sbyte Slot { get; set; } // Verwende sbyte, da Byte vorzeichenbehaftet ist in Minecraft-Protokoll

    public override void Write(Stream stream)
    {
        stream.WriteByte(Slot);
    }
}