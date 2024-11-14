using System.Text;

namespace SharpBlock.Core.Utils;

public static class StreamExtensions
{
    public static int ReadVarInt(this Stream stream)
    {
        int numRead = 0;
        long result = 0;
        byte read;

        while (true)
        {
            int current = stream.ReadByte();
            if (current == -1)
            {
                throw new EndOfStreamException();
            }

            read = (byte)current;
            long value = (read & 0b01111111);
            result |= (value << (7 * numRead));

            numRead++;
            if (numRead > 5)
            {
                throw new FormatException("VarInt is too big");
            }

            if ((read & 0b10000000) == 0)
            {
                break;
            }
        }

        if (result > int.MaxValue || result < int.MinValue)
        {
            throw new FormatException("VarInt is out of range");
        }

        return (int)result;
    }

    public static void WriteVarInt(this Stream stream, int value)
    {
        do
        {
            byte temp = (byte)(value & 0b01111111);
            value >>= 7;
            if (value != 0)
            {
                temp |= 0b10000000;
            }

            stream.WriteByte(temp);
        } while (value != 0);
    }

    public static string ReadString(this Stream stream)
    {
        long length = stream.ReadVarInt();
        if (length < 0 || length > int.MaxValue)
        {
            throw new FormatException("String length is invalid");
        }

        byte[] data = new byte[length];
        int bytesRead = stream.Read(data, 0, (int)length);
        if (bytesRead < length)
        {
            throw new EndOfStreamException("Failed to read complete string");
        }

        return Encoding.UTF8.GetString(data);
    }

    public static void WriteString(this Stream stream, string value)
    {
        byte[] data = Encoding.UTF8.GetBytes(value);
        stream.WriteVarInt(data.Length);
        stream.Write(data, 0, data.Length);
    }

    public static ushort ReadUnsignedShort(this Stream stream)
    {
        int high = stream.ReadByte();
        int low = stream.ReadByte();
        if (high == -1 || low == -1)
        {
            throw new EndOfStreamException();
        }

        return (ushort)((high << 8) | low);
    }

    public static void WriteUnsignedShort(this Stream stream, ushort value)
    {
        stream.WriteByte((byte)((value >> 8) & 0xFF));
        stream.WriteByte((byte)(value & 0xFF));
    }

    public static void WriteUuid(this Stream stream, Guid uuid)
    {
        byte[] uuidBytes = uuid.ToByteArray();
        byte[] bigEndianUuidBytes = new byte[16];
        for (int i = 0; i < 16; i++)
        {
            bigEndianUuidBytes[i] = uuidBytes[15 - i];
        }

        stream.Write(bigEndianUuidBytes, 0, 16);
    }
}