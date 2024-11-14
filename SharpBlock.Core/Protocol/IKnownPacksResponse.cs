namespace SharpBlock.Core.Protocol;

public interface IKnownPacksResponse : IPacket
{
    public int KnownPackCount { get; set; }
    public KnownPacks[] KnownPacks { get; set; }
}

public class KnownPacks
{
    public string Namespace { get; set; }
    public string Id { get; set; }
    public string Version { get; set; }
}