using SharpBlock.Core.Protocol;
using SharpBlock.Core.Utils;

namespace SharpBlock.Protocol.Packets.Play
{
    public class ChatMessagePacket : IPacket
    {
        public int PacketId => 0x02;
        public string Message { get; set; }
        public byte Position { get; set; }
        public Guid Sender { get; set; }

        public void Read(Stream stream)
        {
            // Not used on server side for outgoing messages
        }

        public void Write(Stream stream)
        {
            stream.WriteString(Message);
            stream.WriteByte(Position);
            stream.WriteUuid(Sender);
        }

        public Task HandleAsync(IPacketHandler handler, CancellationToken token = default)
        {
            // No server-side handling necessary for outgoing message
            return Task.CompletedTask;
        }
    }
}