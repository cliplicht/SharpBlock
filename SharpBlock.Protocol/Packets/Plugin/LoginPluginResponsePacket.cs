using SharpBlock.Core.Protocol;

namespace SharpBlock.Protocol.Packets.Plugin;

public class LoginPluginResponsePacket : IPacket
{
    public int PacketId { get; } = 0x02;
    public void Read(Stream stream)
    {
        throw new NotImplementedException();
    }

    public void Write(Stream stream)
    {
        throw new NotImplementedException();
    }

    public Task HandleAsync(IPacketHandler handler, CancellationToken token)
    {
        throw new NotImplementedException();
    }
}