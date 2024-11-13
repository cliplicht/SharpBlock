using SharpBlock.Core.Protocol;

namespace SharpBlock.Protocol.Packets.Ping
{
    public class PongPacket : IPongPacket
    {
        public int PacketId => 0x01;
        public long Payload { get; set; }

        public void Read(Stream stream)
        {
            // Not needed for server-side implementation
        }

        public void Write(Stream stream)
        {
            byte[] buffer = BitConverter.GetBytes(Payload);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(buffer);
            }
            stream.Write(buffer, 0, 8);
        }

        public Task HandleAsync(IPacketHandler handler)
        {
            // Server doesn't handle its own response
            return Task.CompletedTask;
        }
    }
}