using System.Security.Cryptography;
using SharpBlock.Core.Network;
using SharpBlock.Core.Utils;
using SharpBlock.Protocol.Packets.Encryption;

namespace SharpBlock.Protocol.Handlers;

public class LoginHandler
{
    private readonly RSA _serverRsa;
    private readonly byte[] _verifyToken;
    
    public LoginHandler()
    {
        EncryptionHelper.GenerateKeyPair(out _serverRsa);
        _verifyToken = new byte[4];
        RandomNumberGenerator.Fill(_verifyToken);
    }
    
    public async Task SendEncryptionRequest(IClientConnection client)
    {
        var publicKey = _serverRsa.ExportSubjectPublicKeyInfo();

        var encryptionRequestPacket = new EncryptionRequestPacket()
        {
            ServerId = string.Empty,
            PublicKey = publicKey,
            VerifyToken = _verifyToken,
            ShouldAuthenticate = true,
        };
        
        await client.SendPacketAsync(encryptionRequestPacket);
    }
}