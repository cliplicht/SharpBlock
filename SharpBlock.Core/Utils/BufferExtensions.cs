namespace SharpBlock.Core.Utils;

public static class BufferExtensions
{
    public static int ReadVarInt(this byte[] buffer, int offset, out int bytesRead)
    {
        int numRead = 0;
        int result = 0;
        byte read;
        do
        {
            if (offset + numRead >= buffer.Length)
            {
                throw new IndexOutOfRangeException("Not enough bytes to read VarInt");
            }

            read = buffer[offset + numRead];
            int value = (read & 0b01111111);
            result |= (value << (7 * numRead));

            numRead++;
            if (numRead > 5)
            {
                throw new FormatException("VarInt is too big");
            }
        } while ((read & 0b10000000) != 0);

        bytesRead = numRead;
        return result;
    }
}