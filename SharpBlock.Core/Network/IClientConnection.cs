using System.Net;
using SharpBlock.Core.Protocol;

namespace SharpBlock.Core.Network;

public interface IClientConnection
{
    // Existing members
    ConnectionState ConnectionState { get; set; }
    Task SendPacketAsync(IPacket packet);

    // Add the missing methods
    byte[] GetBufferedData();
    void SetBufferedData(byte[] buffer, int offset, int count);
    void ClearBufferedData();

    // Add the missing property
    EndPoint? RemoteEndPoint { get; }
}