namespace SharpBlock.Core.Protocol;

public interface IClientSettingsPacket : IPacket
{
    public string Locale { get; set; }
    public byte ViewDistance { get; set; }
    public int ChatMode { get; set; }
    public bool ChatColors { get; set; }
    public byte DisplayedSkinParts { get; set; }
    public int MainHand { get; set; }
    public bool EnableTextFiltering { get; set; }
    public bool AllowServerListings { get; set; }
}