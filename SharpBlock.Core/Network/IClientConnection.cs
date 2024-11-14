using System.Net;
using SharpBlock.Core.Protocol;

namespace SharpBlock.Core.Network;

public interface IClientConnection
{
    // Existing members
    ConnectionState ConnectionState { get; set; }
    Task SendPacketAsync(IPacket packet);
    EndPoint? RemoteEndPoint { get; }
}