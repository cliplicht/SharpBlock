namespace SharpBlock.Core.Protocol;

public interface IPacket
{
    int PacketId { get; }

    void Read(Stream stream);

    void Write(Stream stream);

    Task HandleAsync(IPacketHandler handler, CancellationToken token);
}