using SharpBlock.Core.Models;
using SharpBlock.Core.Protocol;
using SharpBlock.Core.Utils;
using SharpBlock.Protocol.Handlers;

namespace SharpBlock.Protocol.Packets.Login;
    public class LoginSuccessPacket : IPacket
    {
        public int PacketId => 0x02;

        public MinecraftAccount MinecraftAccount { get; set; }

        public void Read(Stream stream)
        {
            // Not used on server side
        }

        public void Write(Stream stream)
        {
            stream.WriteUuid(MinecraftAccount.Id);
            stream.WriteString(MinecraftAccount.Name);
            stream.WriteVarInt(MinecraftAccount.Properties.Count);

            foreach (var property in MinecraftAccount.Properties)
            {
                stream.WriteString(property.Name);
                stream.WriteString(property.Value);
                stream.WriteByte(property.IsSigned ? (byte)1 : (byte)0);
                if (property.IsSigned)
                {
                    stream.WriteString(property.Signature);
                }
                
            }
        }

        public  Task HandleAsync(IPacketHandler handler,CancellationToken token = default)
        {
            //Nothing to handle   
            return Task.CompletedTask;
        }
    }