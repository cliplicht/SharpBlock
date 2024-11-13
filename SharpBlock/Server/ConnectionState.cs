namespace SharpBlock.Server;

public enum ConnectionState
{
    Handshaking,
    Status,
    Login,
    Configuration,
    Play
}