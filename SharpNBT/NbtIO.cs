using System.IO.Compression;
using SharpNBT.Structs;

namespace SharpNBT;

public static class NbtIO
{
    public static void WriteTo(NbtTag tag, Stream stream, NbtCompression compression)
    {
        switch (compression)
        {
            case NbtCompression.None:
                WriteUncompressed(tag, stream);
                break;

            case NbtCompression.GZip:
                using (var gzipStream = new GZipStream(stream, CompressionLevel.SmallestSize, leaveOpen: true))
                {
                    WriteUncompressed(tag, gzipStream);
                }
                break;

            case NbtCompression.ZLib:
                using (var deflateStream = new DeflateStream(stream, CompressionLevel.SmallestSize, leaveOpen: true))
                {
                    WriteUncompressed(tag, deflateStream);
                }
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(compression), compression, null);
        }
    }

    private static void WriteUncompressed(NbtTag tag, Stream stream)
    {
        // Implement the logic to write the NBT tag data to the stream
        // This involves writing the tag type, name, and payload
        tag.WriteTag(stream);
    }
}