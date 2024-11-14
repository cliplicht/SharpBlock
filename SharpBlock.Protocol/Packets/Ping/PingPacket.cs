using SharpBlock.Core.Protocol;
using SharpBlock.Core.Utils;
using SharpBlock.Protocol.Handlers;

namespace SharpBlock.Protocol.Packets.Ping
{
    public class PingPacket : IPingPacket
    {
        public int PacketId => 0x01;
        public long Payload { get; set; }

        public void Read(Stream stream)
        {
            Payload = stream.ReadVarInt();
        }

        public void Write(Stream stream)
        {
            // Not needed for client request
        }

        public async Task HandleAsync(IPacketHandler handler, CancellationToken cancellationToken = default)
        {
            await handler.HandlePingPacketAsync(this, cancellationToken);
        }
    }
}