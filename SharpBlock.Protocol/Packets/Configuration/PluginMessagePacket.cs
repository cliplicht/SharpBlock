using SharpBlock.Core.Protocol;
using SharpBlock.Core.Utils;

namespace SharpBlock.Protocol.Packets.Configuration;

public class PluginMessagePacket : IPluginMessagePacket
{
    public int PacketId { get; } = 0x02;
    public string Channel { get; set; }
    public byte[] Data { get; set; }
    public void Read(Stream stream)
    {
        Channel = stream.ReadString();
        int remainingLength = (int)(stream.Length - stream.Position);
        Data = new byte[remainingLength];
        stream.Read(Data, 0, remainingLength);
    }

    public void Write(Stream stream)
    {
        throw new NotImplementedException();
    }

    public async Task HandleAsync(IPacketHandler handler, CancellationToken token)
    {
        await handler.HandlePluginMessageAsync(this, token);
    }

    
}