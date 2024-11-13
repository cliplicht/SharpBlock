using SharpNBT.Extensions;

namespace SharpBlock.Pakets;

public class PlayerPositionAndLookPacket : OutgoingPaket
{
    public override int PacketId => 0x39;

    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }
    public float Yaw { get; set; }
    public float Pitch { get; set; }
    public byte Flags { get; set; }
    public int TeleportId { get; set; }
    public bool DismountVehicle { get; set; }

    public override void Write(Stream stream)
    {
        stream.WriteDouble(X);
        stream.WriteDouble(Y);
        stream.WriteDouble(Z);
        stream.WriteFloat(Yaw);
        stream.WriteFloat(Pitch);
        stream.WriteUnsignedByte(Flags);
        stream.WriteVarInt(TeleportId);
        stream.WriteBoolean(DismountVehicle);
    }
}