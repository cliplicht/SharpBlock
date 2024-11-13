namespace SharpNBT.Extensions;

public static class ByteExtensions
{
    /// <summary>
    /// Reads a VarInt from the given buffer starting at the specified offset.
    /// </summary>
    /// <param name="buffer">The buffer containing the VarInt data.</param>
    /// <param name="offset">The starting position in the buffer.</param>
    /// <param name="bytesRead">The number of bytes read from the buffer.</param>
    /// <returns>The decoded integer value.</returns>
    public static int ReadVarInt(this byte[] buffer, int offset, out int bytesRead)
    {
        int numRead = 0;
        int result = 0;
        byte read;

        do
        {
            if (offset + numRead >= buffer.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), "Not enough bytes to read VarInt.");
            }

            read = buffer[offset + numRead];
            int value = read & 0b01111111;
            result |= value << (7 * numRead);

            numRead++;

            if (numRead > 5)
            {
                throw new FormatException("VarInt is too big");
            }
        } while ((read & 0b10000000) != 0);

        bytesRead = numRead;
        return result;
    }
    
    /// <summary>
    /// Liest ein VarInt aus dem gegebenen ReadOnlySpan ab dem angegebenen Offset.
    /// </summary>
    /// <param name="buffer">Der ReadOnlySpan, der die VarInt-Daten enthält.</param>
    /// <param name="offset">Die Startposition im Puffer.</param>
    /// <param name="bytesRead">Die Anzahl der aus dem Puffer gelesenen Bytes.</param>
    /// <returns>Der dekodierte ganzzahlige Wert.</returns>
    public static int ReadVarInt(this ReadOnlySpan<byte> buffer, int offset, out int bytesRead)
    {
        int numRead = 0;
        int result = 0;
        byte read;

        do
        {
            if (offset + numRead >= buffer.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), "Nicht genügend Bytes, um VarInt zu lesen.");
            }

            read = buffer[offset + numRead];
            int value = read & 0b01111111;
            result |= value << (7 * numRead);

            numRead++;

            if (numRead > 5)
            {
                throw new FormatException("VarInt ist zu groß");
            }
        } while ((read & 0b10000000) != 0);

        bytesRead = numRead;
        return result;
    }
}