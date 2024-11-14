namespace SharpBlock.Core.Utils;

public static class BufferExtensions
{
    public static int ReadVarInt(this byte[] buffer, int offset, out int bytesRead)
    {
        bytesRead = 0;
        int numRead = 0;
        int result = 0;
        byte read;

        while (true)
        {
            if (offset + numRead >= buffer.Length)
            {
                // Not enough data to read a complete VarInt
                throw new Exception("ReadVarInt not enough data to read complete VarInt");
            }

            read = buffer[offset + numRead];
            int value = (read & 0b01111111);
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

        bytesRead = numRead;
        return result;
    }
}