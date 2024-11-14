using System.Net;
using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using SharpBlock.Core.Models;
using SharpBlock.Core.Utils;

namespace SharpBlock.Core.Services;

public class EncryptionService
{
    private readonly ILogger<EncryptionService> _logger;
    private readonly HttpClient _httpClient;
    public RSA Rsa { get; set; }
    public byte[] PublicKey { get; set; }
    public byte[] VerifyToken { get; set; } = new byte[4];
    private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions { Converters = { new FlexibleGuidConverter() }};
    
    public EncryptionService(ILogger<EncryptionService> logger, HttpClient httpClient)
    {
        _logger = logger;
        _httpClient = httpClient;
        Rsa = RSA.Create();
        PublicKey = Rsa.ExportSubjectPublicKeyInfo();
        RandomNumberGenerator.Fill(VerifyToken);
    }


    public async Task<MinecraftAccount?> ValidateMinecraftAccount(byte[] sharedSecret, string username, string? ip = null)
    {
        var serverHash = Utils.Utils.GenerateServerHash(sharedSecret, PublicKey);
        
        
        var requestUri = new Uri($"https://sessionserver.mojang.com/session/minecraft/hasJoined?username={username}&serverId={serverHash}");

        var request = new HttpRequestMessage()
        {
            Method = HttpMethod.Get,
            RequestUri = requestUri
        };
        
        var response = await _httpClient.SendAsync(request);

        if (response.StatusCode != HttpStatusCode.OK)
        {
            _logger.LogError("Failed to validate minecraft account ({Username}): {StatusCode} | {Message}", username, response.StatusCode.ToString(), await response.Content.ReadAsStringAsync());
            return null;
        }

        var responseString = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<MinecraftAccount>(responseString, _jsonSerializerOptions);
    }
    
    
    public Task PrintDebugInfo()
    {
        string privateKey = Convert.ToBase64String(Rsa.ExportRSAPrivateKey());
        string publicKey = Convert.ToBase64String(Rsa.ExportSubjectPublicKeyInfo());
        string verifyToken = Convert.ToBase64String(VerifyToken);
        
        _logger.LogDebug("Encryption Details:");
        _logger.LogDebug("PrivateKey: {PrivateKey}", privateKey);
        _logger.LogDebug("PublicKey: {PublicKey}", publicKey);
        _logger.LogDebug("VerifyToken: {VerifyToken}", verifyToken);
        
        return Task.CompletedTask;
    }
    
    
    
}