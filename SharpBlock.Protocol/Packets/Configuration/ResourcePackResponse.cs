using SharpBlock.Core.Protocol;
using SharpBlock.Core.Utils;

namespace SharpBlock.Protocol.Packets.Configuration;

public class ResourcePackResponse : IResourcePackPacket
{
    public int PacketId { get; } = 0x06;
    public string ResourcePackId { get; set; }
    public ResourcePackResult Result { get; set; }
    public void Read(Stream stream)
    {
        ResourcePackId = stream.ReadString();
        Result = (ResourcePackResult)stream.ReadVarInt();
    }

    public void Write(Stream stream)
    {
        throw new NotImplementedException();
    }

    public async Task HandleAsync(IPacketHandler handler, CancellationToken token)
    {
        await Task.CompletedTask;
        //await handler.
    }

    
}