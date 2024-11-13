namespace SharpBlock.Core.Protocol;

public interface IStatusResponsePacket : IPacket
{
    public string JsonResponse { get; set; }
}