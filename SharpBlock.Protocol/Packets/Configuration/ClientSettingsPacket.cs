using SharpBlock.Core.Protocol;
using SharpBlock.Core.Utils;

namespace SharpBlock.Protocol.Packets.Play;

public class ClientSettingsPacket : IClientSettingsPacket
{
    public int PacketId { get; } = 0x02;
    public string Locale { get; set; }
    public byte ViewDistance { get; set; }
    public int ChatMode { get; set; }
    public bool ChatColors { get; set; }
    public byte DisplayedSkinParts { get; set; }
    public int MainHand { get; set; }
    public bool EnableTextFiltering { get; set; }
    public bool AllowServerListings { get; set; }

    public void Read(Stream stream)
    {
        Locale = stream.ReadString();
        ViewDistance = (byte)stream.ReadByte();
        ChatMode = stream.ReadVarInt();
        ChatColors = stream.ReadByte() != 0;
        DisplayedSkinParts = (byte)stream.ReadByte();
        MainHand = stream.ReadVarInt();
        EnableTextFiltering = stream.ReadByte() != 0;
        AllowServerListings = stream.ReadByte() != 0;
    }

    public void Write(Stream stream)
    {
       
    }

    public async Task HandleAsync(IPacketHandler handler, CancellationToken cancellationToken)
    {
        await handler.HandleClientSettingsAsync(this, cancellationToken);
    }

    
}