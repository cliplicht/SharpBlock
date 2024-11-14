namespace SharpBlock.Core.Protocol;

public interface IEncryptionResponsePacket : IPacket
{
    public byte[] EncryptedSharedSecret { get; set; }
    public byte[] EncryptedVerifyToken { get; set; }
}