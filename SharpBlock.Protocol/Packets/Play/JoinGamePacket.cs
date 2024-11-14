using SharpBlock.Core.Protocol;
using SharpBlock.Core.Utils;

namespace SharpBlock.Protocol.Packets.Play;

public class JoinGamePacket : IJoinGamePacket
{
    public int PacketId { get; }
    public int EntityId { get; set; }
    public byte GameMode { get; set; }
    public int Dimension { get; set; }
    public long HashedSeed { get; set; }
    public int MaxPlayers { get; set; }
    public string LevelType { get; set; } = "default";
    public int ViewDistance { get; set; }
    public bool ReducedDebugInfo { get; set; }
    public void Read(Stream stream)
    {
        // Not needed on server side, so we leave this empty
    }

    public void Write(Stream stream)
    {
        stream.WriteInt(EntityId);
        stream.WriteByte(GameMode);
        stream.WriteInt(Dimension);
        stream.WriteLong(HashedSeed);
        stream.WriteInt(MaxPlayers);
        stream.WriteString(LevelType);
        stream.WriteVarInt(ViewDistance);
        stream.WriteBool(ReducedDebugInfo);
    }

    public async Task HandleAsync(IPacketHandler handler, CancellationToken token)
    {
        // Not required; server only sends this packet
        await Task.CompletedTask;
    }

   
}