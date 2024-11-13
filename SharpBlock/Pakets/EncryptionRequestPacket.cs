using SharpNBT.Extensions;

namespace SharpBlock.Pakets;

public class EncryptionRequestPacket : OutgoingPaket
{
    public override int PacketId { get; } = 0x01;
    public string ServerId { get; set; } = "";
    public byte[] PublicKey { get; set; } = Array.Empty<byte>();
    public byte[] VerifyToken { get; set; } = Array.Empty<byte>();
    public bool ShouldAuthenticate { get; set; } = true;

    public override void Write(Stream stream)
    {
        stream.WriteStringMc(ServerId);
        stream.WriteVarInt(PublicKey.Length);
        stream.Write(PublicKey, 0, PublicKey.Length);
        stream.WriteVarInt(VerifyToken.Length);
        stream.Write(VerifyToken, 0, VerifyToken.Length);
        stream.WriteBoolean(ShouldAuthenticate);
    }
}