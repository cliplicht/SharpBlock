using Microsoft.Extensions.Logging;
using SharpBlock.Core;
using SharpBlock.Core.Extensions;
using SharpBlock.Core.Protocol;
using SharpBlock.Core.Utils;

namespace SharpBlock.Protocol;

public class PacketParser
{
    private readonly ILogger<PacketParser> _logger;
    private readonly PacketFactory _packetFactory;

    public PacketParser(ILogger<PacketParser> logger, PacketFactory packetFactory)
    {
        _logger = logger;
        _packetFactory = packetFactory;
    }

    public List<IPacket> Parse(byte[] buffer, int bytesRead, ConnectionState connectionState, ref int leftoverBytes)
{
    List<IPacket> packets = new();
    int offset = 0;

    _logger.LogInformation($"Starting packet parsing with {bytesRead} bytes, connection state: {connectionState}");

    while (offset < bytesRead)
    {
        int varIntLength;

        // Try to read packet length
        var packetLengthValue = buffer.ReadVarInt(offset, out varIntLength);
        if (packetLengthValue == null)
        {
            _logger.LogWarning("Incomplete VarInt for packet length detected.");
            leftoverBytes = bytesRead - offset;
            break; // Wait for more data
        }

        int packetLength = packetLengthValue;
        offset += varIntLength;

        if (offset + packetLength > bytesRead)
        {
            _logger.LogWarning($"Incomplete packet detected. Packet length: {packetLength}, bytes available: {bytesRead - offset}");
            leftoverBytes = bytesRead - (offset - varIntLength);
            break;
        }

        // Try to read packet ID
        var packetIdValue = buffer.ReadVarInt(offset, out int packetIdLength);
        if (packetIdValue == null)
        {
            _logger.LogWarning("Incomplete VarInt for packet ID detected.");
            leftoverBytes = bytesRead - (offset - varIntLength);
            break; // Wait for more data
        }

        int packetId = packetIdValue;
        offset += packetIdLength;

        IPacket packet = _packetFactory.CreatePacket(packetId, connectionState);
        if (packet == null)
        {
            _logger.LogWarning($"Unknown packet ID {packetId} in state {connectionState}");
            // Skip unknown packet
            offset += packetLength - packetIdLength;
            continue;
        }

        int packetDataLength = packetLength - packetIdLength;
        if (offset + packetDataLength > bytesRead)
        {
            _logger.LogWarning("Incomplete packet data detected.");
            leftoverBytes = bytesRead - (offset - varIntLength);
            break; // Wait for more data
        }

        using var ms = new MemoryStream(buffer, offset, packetDataLength);
        packet.Read(ms);
        packets.Add(packet);

        _logger.LogInformation($"Successfully parsed packet ID {packetId} in state {connectionState}");

        offset += packetDataLength;
    }

    return packets;
}
}