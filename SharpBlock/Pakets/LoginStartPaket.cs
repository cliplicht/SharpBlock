using SharpBlock.Server;
using SharpNBT.Extensions;

namespace SharpBlock.Pakets
{
    public class LoginStartPaket : IncomingPaket
    {
        public override int PacketId => 0x00;

        public string PlayerName { get; private set; } = null!;
        public override void Read(Stream stream)
        {
            PlayerName = stream.ReadString();
        }

        public override void Handle(PacketHandler handler)
        {
            handler.HandleLoginStart(this);
        }
    }
}