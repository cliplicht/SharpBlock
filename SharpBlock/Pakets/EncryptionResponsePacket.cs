using SharpBlock.Server;
using SharpNBT.Extensions;

namespace SharpBlock.Pakets;

public class EncryptionResponsePacket : IncomingPaket
{
    public override int PacketId { get; } = 0x01;

    public byte[] EncryptedSharedSecret { get; set; } = Array.Empty<byte>();
    public byte[] EncryptedVerifyToken { get; set; } = Array.Empty<byte>();
    
    public override void Read(Stream stream)
    {
        EncryptedSharedSecret = stream.ReadByteArrayWithLength();
        EncryptedVerifyToken = stream.ReadByteArrayWithLength();
    }

    public override void Handle(PacketHandler handler)
    { 
        handler.HandleEncryptionResponse(this);
    }
}