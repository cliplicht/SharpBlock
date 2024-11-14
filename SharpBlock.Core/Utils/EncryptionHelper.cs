using System.Security.Cryptography;

namespace SharpBlock.Core.Utils;

public class EncryptionHelper
{
    public static RSAParameters GenerateKeyPair(out RSA rsa)
    {
        rsa = RSA.Create();
        return rsa.ExportParameters(true);
    }
}