namespace SharpBlock.Core.Protocol;

public interface IResourcePackPacket : IPacket
{
    public string ResourcePackId { get; set; }
    public ResourcePackResult Result { get; set; }
}

public enum ResourcePackResult
{
    SuccessfullyDownloaded = 0,
    Declined = 1,
    FailedToDownload = 2,
    Accepted = 3,
    Downloaded = 4,
    InvalidUrl = 5,
    FailedToReload = 6,
    Discarded = 7
}