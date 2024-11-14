using SharpBlock.Core.Protocol;

namespace SharpBlock.Protocol.Packets.Login;

public class LoginAcknowledgePacket : IPacket
{
    public int PacketId { get; } = 0x03;

    public void Read(Stream stream)
    {
    }

    public void Write(Stream stream)
    {
    }

    public async Task HandleAsync(IPacketHandler handler, CancellationToken token)
    {
        await handler.HandleLoginAcknowledgedAsync(this, token);
    }
}