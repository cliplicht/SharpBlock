using System.Text;
using SharpBlock.Core.Protocol;
using SharpBlock.Core.Utils;
using SharpBlock.Protocol.Handlers;

namespace SharpBlock.Protocol.Packets.Status;

public class StatusResponsePacket : IStatusResponsePacket
{
    public int PacketId { get; } = 0x00;
    public string JsonResponse { get; set; }
    
    public void Read(Stream stream)
    {
        throw new NotImplementedException();
    }

    public void Write(Stream stream)
    {
        byte[] jsonBytes = Encoding.UTF8.GetBytes(JsonResponse);
        stream.WriteVarInt(jsonBytes.Length);
        stream.Write(jsonBytes, 0, jsonBytes.Length);
    }

    public Task HandleAsync(IPacketHandler handler)
    {
        return Task.CompletedTask;
    }

    
}