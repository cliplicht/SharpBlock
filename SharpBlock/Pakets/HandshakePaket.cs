using SharpBlock.Server;
using SharpNBT.Extensions;
using StreamExtensions = SharpNBT.Extensions.StreamExtensions;

namespace SharpBlock.Pakets;

public class HandshakePaket : IncomingPaket
{
    public override int PacketId => 0x00;

    public int ProtocolVersion { get; private set; }
    public string ServerAddress { get; private set; } = null!;
    public ushort ServerPort { get; private set; }
    public int NextState { get; private set; }

    public override void Read(Stream stream)
    {
        ProtocolVersion = stream.ReadVarInt();
        ServerAddress = stream.ReadString();
        ServerPort = stream.ReadUnsignedShort();
        NextState = stream.ReadVarInt();
    }

    public override void Handle(PacketHandler handler)
    {
        handler.HandleHandshake(this);
    }
}