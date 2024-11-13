using SharpBlock.Core.Protocol;
using SharpBlock.Core.Utils;
using SharpBlock.Protocol.Handlers;

namespace SharpBlock.Protocol.Packets.Login;
    public class LoginSuccessPacket : IPacket
    {
        public int PacketId => 0x02;

        public Guid UUID { get; set; }
        public string Username { get; set; }

        public void Read(Stream stream)
        {
            // Not used on server side
        }

        public void Write(Stream stream)
        {
            stream.WriteUuid(UUID);
            stream.WriteString(Username);
        }

        public  Task HandleAsync(IPacketHandler handler)
        {
            //Nothing to handle   
            return Task.CompletedTask;
        }
    }