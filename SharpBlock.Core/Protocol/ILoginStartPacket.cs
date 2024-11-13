namespace SharpBlock.Core.Protocol;

public interface ILoginStartPacket : IPacket
{
    string Username { get; set; }
}