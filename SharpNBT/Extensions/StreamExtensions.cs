using System.Security.Cryptography;
using System.Text;

namespace SharpNBT.Extensions
{
    public static class StreamExtensions
    {
        public static int ReadVarInt(this Stream stream)
        {
            int numRead = 0;
            int result = 0;
            byte read;

            do
            {
                int byteRead = stream.ReadByte();
                if (byteRead == -1)
                {
                    throw new EndOfStreamException();
                }

                read = (byte)byteRead;
                int value = (read & 0b01111111);
                result |= (value << (7 * numRead));

                numRead++;
                if (numRead > 5)
                {
                    throw new InvalidOperationException("VarInt is too big.");
                }
            } while ((read & 0b10000000) != 0);

            return result;
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
            int length = stream.ReadVarInt();
            byte[] buffer = new byte[length];
            int read = stream.Read(buffer, 0, length);
            if (read < length)
            {
                throw new EndOfStreamException();
            }
            return Encoding.UTF8.GetString(buffer);
        }

        public static void WriteString(this Stream stream, string value)
        {
            byte[] data = Encoding.UTF8.GetBytes(value);
            stream.WriteShort((short)data.Length); // NBT uses a short for string lengths
            stream.Write(data, 0, data.Length);
        }
        
        public static void WriteStringMc(this Stream stream, string value)
        {
            byte[] data = Encoding.UTF8.GetBytes(value);
            stream.WriteVarInt(data.Length); // Correct: Using VarInt for string length
            stream.Write(data, 0, data.Length);
        }

        public static void WriteShort(this Stream stream, short value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }
            stream.Write(bytes, 0, bytes.Length);
        }

        public static ushort ReadUnsignedShort(this Stream stream)
        {
            int highByte = stream.ReadByte();
            int lowByte = stream.ReadByte();

            if (highByte == -1 || lowByte == -1)
            {
                throw new EndOfStreamException("Cannot read unsigned short from stream.");
            }

            return (ushort)((highByte << 8) | lowByte);
        }

        public static void WriteUnsignedShort(this Stream stream, ushort value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }
            stream.Write(bytes, 0, bytes.Length);
        }

        public static void WriteInt(this Stream stream, int value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }
            stream.Write(bytes, 0, bytes.Length);
        }

        public static void WriteLong(this Stream stream, long value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }
            stream.Write(bytes, 0, bytes.Length);
        }

        public static void WriteFloat(this Stream stream, float value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }
            stream.Write(bytes, 0, bytes.Length);
        }

        public static void WriteDouble(this Stream stream, double value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }
            stream.Write(bytes, 0, bytes.Length);
        }

        public static void WriteBoolean(this Stream stream, bool value)
        {
            stream.WriteByte((byte)(value ? 1 : 0));
        }

        public static void WriteUnsignedByte(this Stream stream, byte value)
        {
            stream.WriteByte(value);
        }

        public static void WriteByte(this Stream stream, sbyte value)
        {
            stream.WriteByte((byte)value);
        }

        public static void WriteByteArrayWithLength(this Stream stream, byte[] data)
        {
            stream.WriteVarInt(data.Length);
            stream.Write(data, 0, data.Length);
        }

        public static long ReadLong(this Stream stream)
        {
            byte[] data = new byte[8];
            int read = stream.Read(data, 0, 8);
            if (read < 8)
                throw new EndOfStreamException("Failed to read long value.");

            if (BitConverter.IsLittleEndian)
                Array.Reverse(data);

            return BitConverter.ToInt64(data, 0);
        }

        public static int ReadInt(this Stream stream)
        {
            byte[] data = new byte[4];
            int read = stream.Read(data, 0, 4);
            if (read < 4)
                throw new EndOfStreamException("Failed to read int value.");

            if (BitConverter.IsLittleEndian)
                Array.Reverse(data);

            return BitConverter.ToInt32(data, 0);
        }

        public static short ReadShort(this Stream stream)
        {
            byte[] data = new byte[2];
            int read = stream.Read(data, 0, 2);
            if (read < 2)
                throw new EndOfStreamException("Failed to read short value.");

            if (BitConverter.IsLittleEndian)
                Array.Reverse(data);

            return BitConverter.ToInt16(data, 0);
        }

        public static float ReadFloat(this Stream stream)
        {
            byte[] data = new byte[4];
            int read = stream.Read(data, 0, 4);
            if (read < 4)
                throw new EndOfStreamException("Failed to read float value.");

            if (BitConverter.IsLittleEndian)
                Array.Reverse(data);

            return BitConverter.ToSingle(data, 0);
        }

        public static double ReadDouble(this Stream stream)
        {
            byte[] data = new byte[8];
            int read = stream.Read(data, 0, 8);
            if (read < 8)
                throw new EndOfStreamException("Failed to read double value.");

            if (BitConverter.IsLittleEndian)
                Array.Reverse(data);

            return BitConverter.ToDouble(data, 0);
        }
        
        public static byte[] ReadByteArrayWithLength(this Stream stream)
        {
            int length = stream.ReadVarInt(); // Read the length as a VarInt
            if (length < 0)
            {
                throw new InvalidOperationException("Invalid length for byte array.");
            }

            byte[] data = new byte[length];
            int bytesRead = stream.Read(data, 0, length);
            if (bytesRead < length)
            {
                throw new EndOfStreamException("Unable to read the complete byte array: reached end of stream.");
            }

            return data;
        }
    }
}