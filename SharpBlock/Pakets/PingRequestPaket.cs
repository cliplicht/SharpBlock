using SharpBlock.Server;
using SharpNBT.Extensions;

namespace SharpBlock.Pakets
{
    public class PingRequestPaket : IncomingPaket
    {
        public override int PacketId => 0x01;

        public long Payload { get; private set; }

        public override void Read(Stream stream)
        {
            Payload = stream.ReadLong();
        }

        public override void Handle(PacketHandler handler)
        {
            handler.HandlePingRequest(this);
        }
    }
}