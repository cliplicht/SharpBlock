namespace SharpBlock.Core;

public enum ConnectionState
{
    Handshaking = 0,
    Status = 1,
    Login = 2,
    Configuration = 3,
    Play = 4
}