using Microsoft.Extensions.Logging;
using SharpBlock.Core;
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

    public List<IPacket> Parse(byte[] buffer, int offset, int length, ConnectionState connectionState,
        out int bytesConsumed)
    {
        List<IPacket> packets = new();
        int initialOffset = offset;
        int bytesRead = 0;

        _logger.LogInformation(
            $"Starting packet parsing with {length} bytes at offset {offset}, connection state: {connectionState}");

        while (bytesRead < length)
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
            bytesRead += packetLengthVarIntLength;

            if (bytesRead + packetLength > length)
            {
                _logger.LogWarning(
                    $"Incomplete packet detected. Packet length: {packetLength}, bytes available: {length - bytesRead}");
                // Reset offset to start of incomplete packet
                offset = packetStartOffset;
                bytesRead -= packetLengthVarIntLength;
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
                bytesRead -= packetLengthVarIntLength;
                break; // Wait for more data
            }

            int packetId = packetIdValue;
            offset += packetIdVarIntLength;
            bytesRead += packetIdVarIntLength;

            // Calculate bytes read from packet data so far
            int bytesReadFromPacketData = offset - packetDataStartOffset;

            // Correct packet data length calculation
            int packetDataLength = packetLength - bytesReadFromPacketData;

            if (packetDataLength < 0)
            {
                _logger.LogWarning("Packet data length is negative.");
                // Reset offset to start of incomplete packet
                offset = packetStartOffset;
                bytesRead -= (packetLengthVarIntLength + packetIdVarIntLength);
                break;
            }

            if (bytesRead + packetDataLength > length)
            {
                _logger.LogWarning("Incomplete packet data detected.");
                // Reset offset to start of incomplete packet
                offset = packetStartOffset;
                bytesRead -= (packetLengthVarIntLength + packetIdVarIntLength);
                break; // Wait for more data
            }

            IPacket packet = _packetFactory.CreatePacket(packetId, connectionState);
            if (packet == null)
            {
                _logger.LogWarning($"Unknown packet ID {packetId} in state {connectionState}");
                // Skip unknown packet
                offset += packetDataLength;
                bytesRead += packetDataLength;
                continue;
            }

            // Read the packet data
            using var ms = new MemoryStream(buffer, offset, packetDataLength);
            packet.Read(ms);
            packets.Add(packet);

            _logger.LogInformation($"Successfully parsed packet ID {packetId} in state {connectionState}");

            // Advance the offset and bytesRead by the packet data length
            offset += packetDataLength;
            bytesRead += packetDataLength;
        }

        // Set the number of bytes consumed
        bytesConsumed = offset - initialOffset;

        return packets;
    }
}