using SharpBlock.Server;

namespace SharpBlock.Pakets
{
    public interface IPaket
    {
        int PacketId { get; }

        void Read(Stream stream);
        void Write(Stream stream);
        void Handle(PacketHandler handler);
    }
}