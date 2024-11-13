using SharpBlock.Server;

namespace SharpBlock.Pakets;

public abstract class IncomingPaket : IPaket
{
    public abstract int PacketId { get; }

    public abstract void Read(Stream stream);

    public virtual void Handle(PacketHandler handler)
    {
        // Standardverhalten oder abstrakt, wenn zwingend zu implementieren
    }

    public void Write(Stream stream)
    {
        throw new NotSupportedException("Cannot write an incoming packet.");
    }
}