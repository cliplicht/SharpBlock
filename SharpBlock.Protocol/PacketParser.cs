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
            int packetStartOffset = offset;

            // Read packet length VarInt
            var packetLengthValue = buffer.ReadVarInt(offset, out int packetLengthVarIntLength);
            if (packetLengthValue == null)
            {
                _logger.LogWarning("Incomplete VarInt for packet length detected.");
                break; // Wait for more data
            }

            int packetLength = packetLengthValue;
            offset += packetLengthVarIntLength;

            if (offset + packetLength > bytesRead)
            {
                _logger.LogWarning(
                    $"Incomplete packet detected. Packet length: {packetLength}, bytes available: {bytesRead - offset}");
                // Reset offset to start of incomplete packet
                offset = packetStartOffset;
                break;
            }

            int packetDataStartOffset = offset;

            // Read packet ID VarInt
            var packetIdValue = buffer.ReadVarInt(offset, out int packetIdVarIntLength);
            if (packetIdValue == null)
            {
                _logger.LogWarning("Incomplete VarInt for packet ID detected.");
                // Reset offset to start of incomplete packet
                offset = packetStartOffset;
                break; // Wait for more data
            }

            int packetId = packetIdValue;
            offset += packetIdVarIntLength;

            // Calculate bytes read from packet data so far
            int bytesReadFromPacketData = offset - packetDataStartOffset;

            // Correct packet data length calculation
            int packetDataLength = packetLength - bytesReadFromPacketData;

            if (packetDataLength < 0)
            {
                _logger.LogWarning("Packet data length is negative.");
                // Reset offset to start of incomplete packet
                offset = packetStartOffset;
                break;
            }

            if (offset + packetDataLength > bytesRead)
            {
                _logger.LogWarning("Incomplete packet data detected.");
                // Reset offset to start of incomplete packet
                offset = packetStartOffset;
                break; // Wait for more data
            }

            IPacket packet = _packetFactory.CreatePacket(packetId, connectionState);
            if (packet == null)
            {
                _logger.LogWarning($"Unknown packet ID {packetId} in state {connectionState}");
                // Skip unknown packet
                offset += packetDataLength;
                continue;
            }

            // Read the packet data
            using var ms = new MemoryStream(buffer, offset, packetDataLength);
            packet.Read(ms);
            packets.Add(packet);

            _logger.LogInformation($"Successfully parsed packet ID {packetId} in state {connectionState}");

            // Advance the offset by the packet data length
            offset += packetDataLength;
        }

        // Calculate leftover bytes
        leftoverBytes = bytesRead - offset;

        return packets;
    }
}