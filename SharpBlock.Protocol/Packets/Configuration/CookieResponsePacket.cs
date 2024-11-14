using SharpBlock.Core.Protocol;
using SharpBlock.Core.Utils;

namespace SharpBlock.Protocol.Packets.Cookie;

public class CookieResponsePacket : IPacket
{
    public int PacketId { get; } = 0x03;
    public string Identifier { get; set; }
    public bool HasPayload { get; set; }
    public int PayloadLength { get; set; }
    public byte[] Payload { get; set; }
    public void Read(Stream stream)
    {
        Identifier = stream.ReadString();
        HasPayload = stream.ReadByte() != 0;
        PayloadLength = stream.ReadByte();
        Payload = new byte[PayloadLength];
        stream.Read(Payload, 0, PayloadLength);
    }

    public void Write(Stream stream)
    {
        
    }

    public async Task HandleAsync(IPacketHandler handler, CancellationToken token)
    {
        await handler.HandleCookieResponseAsync(this, token);
    }
}