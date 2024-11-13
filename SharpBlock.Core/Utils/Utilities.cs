using System.Numerics;
using System.Security.Cryptography;
using System.Text;

namespace SharpBlock.Core.Utils;

public static class Utilities
{
    public static string GenerateServerHash(byte[] sharedSecret, byte[] publicKey)
    {
        using (SHA1 sha1 = SHA1.Create())
        {
            sha1.TransformBlock(Encoding.UTF8.GetBytes(""), 0, 0, null, 0);
            sha1.TransformBlock(sharedSecret, 0, sharedSecret.Length, null, 0);
            sha1.TransformFinalBlock(publicKey, 0, publicKey.Length);

            byte[] hash = sha1.Hash;
            BigInteger num = new BigInteger(hash.Reverse().Concat(new byte[] { 0 }).ToArray());
            return num.ToString("x");
        }
    }
}