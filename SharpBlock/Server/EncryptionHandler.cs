using System.Security.Cryptography;

namespace SharpBlock.Server;

public class EncryptionHandler
{
    public RSACryptoServiceProvider Rsa { get; set; }
    public byte[] PublicKey { get; private set; }

    public EncryptionHandler()
    {
        Rsa = new RSACryptoServiceProvider(2048);
        PublicKey = Rsa.ExportSubjectPublicKeyInfo();
    }

    public byte[] DecryptData(byte[] data)
    {
        return Rsa.Decrypt(data, RSAEncryptionPadding.Pkcs1);
    }
}