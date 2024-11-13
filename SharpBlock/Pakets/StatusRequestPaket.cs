using SharpBlock.Server;

namespace SharpBlock.Pakets
{
    public class StatusRequestPaket : IncomingPaket
    {
        public override int PacketId => 0x00;

        public override void Read(Stream stream)
        {
            // No fields to read for Status Request
        }

        public override void Handle(PacketHandler handler)
        {
            handler.HandleStatusRequest(this);
        }
    }
}