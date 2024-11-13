using SharpBlock.Server;

namespace SharpBlock.Pakets;

public abstract class OutgoingPaket : IPaket
{
    public abstract int PacketId { get; }

    public void Read(Stream stream)
    {
        throw new NotSupportedException("Cannot read an outgoing packet.");
    }

    public abstract void Write(Stream stream);
    public void Handle(PacketHandler handler)
    {
        throw new NotSupportedException("Cannot handle an outgoing packet.");
    }
}