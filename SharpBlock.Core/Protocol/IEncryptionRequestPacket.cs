namespace SharpBlock.Core.Protocol;

public interface IEncryptionRequestPacket : IPacket
{
    public byte[] PublicKey { get; set; }
    public byte[] VerifyToken { get; set; }
}