using System.Security.Cryptography;
using System.Text;

namespace SharpBlock.Core.Extensions;

public static class GuidExtensions
{
    public static Guid NewMinecraftGuid(this Guid guid, string str)
    {
        // SHA-1 Hashing des Strings
        var bytes = Encoding.UTF8.GetBytes(str);
        var hash = SHA1.Create().ComputeHash(bytes);

        // Nutze nur die ersten 16 Bytes
        var truncatedHash = new byte[16];
        Array.Copy(hash, truncatedHash, 16);

        // Setze Version 4 (randomisierte UUID)
        truncatedHash[6] = (byte)((truncatedHash[6] & 0x0F) | 0x40);

        // Setze Variante 2 (Leach-Salz-Variante)
        truncatedHash[8] = (byte)((truncatedHash[8] & 0x3F) | 0x80);

        return new Guid(truncatedHash);
    }
}