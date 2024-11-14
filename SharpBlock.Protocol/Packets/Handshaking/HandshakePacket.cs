using SharpBlock.Core.Protocol;
using SharpBlock.Core.Utils;

namespace SharpBlock.Protocol.Packets.Handshaking;

public class HandshakePacket : IHandshakePacket
{
    public int PacketId => 0x00;

    public int ProtocolVersion { get; set; }
    public string ServerAddress { get; set; }
    public ushort ServerPort { get; set; }
    public int NextState { get; set; }

    public void Read(Stream stream)
    {
        ProtocolVersion = stream.ReadVarInt();
        ServerAddress = stream.ReadString();
        ServerPort = stream.ReadUnsignedShort();
        NextState = stream.ReadVarInt();
    }

    public void Write(Stream stream)
    {
        stream.WriteVarInt(ProtocolVersion);
        stream.WriteString(ServerAddress);
        stream.WriteUnsignedShort(ServerPort);
        stream.WriteVarInt(NextState);
    }

    public async Task HandleAsync(IPacketHandler handler, CancellationToken cancellationToken = default)
    {
        await handler.HandleHandshakeAsync(this, cancellationToken);
    }
}