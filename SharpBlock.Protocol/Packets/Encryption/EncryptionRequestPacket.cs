using SharpBlock.Core.Protocol;
using SharpBlock.Core.Utils;

namespace SharpBlock.Protocol.Packets.Encryption;

public class EncryptionRequestPacket : IEncryptionRequestPacket
{
    public int PacketId { get; } = 0x01;
    public byte[] PublicKey { get; set; } = [];
    public byte[] VerifyToken { get; set; } = [];
    public string ServerId { get; set; } = string.Empty;
    public bool ShouldAuthenticate { get; set; } = true;
    
    public void Read(Stream stream)
    {
        throw new NotImplementedException();
    }

    public void Write(Stream stream)
    {
        stream.WriteString(ServerId);
        stream.WriteVarInt(PublicKey.Length);
        stream.Write(PublicKey, 0, PublicKey.Length);
        stream.WriteVarInt(VerifyToken.Length);
        stream.Write(VerifyToken, 0, VerifyToken.Length);
        stream.WriteByte((byte)(ShouldAuthenticate ? 1 : 0));
    }

    public Task HandleAsync(IPacketHandler handler, CancellationToken token)
    {
        return Task.CompletedTask;
    }

    
}