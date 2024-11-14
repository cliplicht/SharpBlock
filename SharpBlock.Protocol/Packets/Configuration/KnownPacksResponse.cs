using SharpBlock.Core.Protocol;
using SharpBlock.Core.Utils;

namespace SharpBlock.Protocol.Packets.Configuration;

public class KnownPacksResponse : IKnownPacksResponse
{
    public int PacketId { get; } = 0x07;
    public int KnownPackCount { get; set; }
    public KnownPacks[] KnownPacks { get; set; }
    public void Read(Stream stream)
    {
        KnownPackCount = stream.ReadVarInt();
        KnownPacks = new KnownPacks[KnownPackCount];
        for (int i = 0; i < KnownPackCount; i++)
        {
            var knownPack = new KnownPacks()
            {
                Namespace = stream.ReadString(),
                Id = stream.ReadString(),
                Version = stream.ReadString()
            };
            KnownPacks[i] = knownPack;
        }
    }

    public void Write(Stream stream)
    {
        throw new NotImplementedException();
    }

    public async Task HandleAsync(IPacketHandler handler, CancellationToken token)
    {
        await handler.HandleKnownPacksAsync(this, token);
    }

    
}