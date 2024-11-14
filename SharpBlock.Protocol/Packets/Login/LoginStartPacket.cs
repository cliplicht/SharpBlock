using SharpBlock.Core.Protocol;
using SharpBlock.Core.Utils;
using SharpBlock.Protocol.Handlers;

namespace SharpBlock.Protocol.Packets.Login
{
    public class LoginStartPacket : ILoginStartPacket
    {
        public int PacketId => 0x00;
        public string Username { get; set; } = string.Empty;

        public void Read(Stream stream)
        {
            Username = stream.ReadString();
        }

        public void Write(Stream stream)
        {
            // Not needed for client request
        }

        public async Task HandleAsync(IPacketHandler handler, CancellationToken token)
        {
            await handler.HandleLoginStartAsync(this,token);
        }
    }
}