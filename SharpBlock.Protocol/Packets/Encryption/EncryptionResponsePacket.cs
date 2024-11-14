using SharpBlock.Core.Protocol;
using SharpBlock.Core.Utils;

namespace SharpBlock.Protocol.Packets.Encryption;

public class EncryptionResponsePacket : IEncryptionResponsePacket
{
    public int PacketId { get; } = 0x01;
    public byte[] EncryptedSharedSecret { get; set; } = Array.Empty<byte>();
    public byte[] EncryptedVerifyToken { get; set; } = Array.Empty<byte>();
    public void Read(Stream stream)
    {
        int sharedSecretLength = stream.ReadVarInt();
        EncryptedSharedSecret = new byte[sharedSecretLength];
        stream.Read(EncryptedSharedSecret, 0, sharedSecretLength);

        int verifyTokenLength = stream.ReadVarInt();
        EncryptedVerifyToken = new byte[verifyTokenLength];
        stream.Read(EncryptedVerifyToken, 0, verifyTokenLength);
    }

    public void Write(Stream stream)
    {
        throw new NotImplementedException();
    }

    public async Task HandleAsync(IPacketHandler handler, CancellationToken token)
    {
        await handler.HandleEncryptionRequestAsync(this, token);
    }
}