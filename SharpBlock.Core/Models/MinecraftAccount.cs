using System.Security.Cryptography;
using System.Text.Json.Serialization;

namespace SharpBlock.Core.Models;

public class MinecraftAccount
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }
    [JsonPropertyName("name")]
    public string Name { get; set; }
    [JsonPropertyName("properties")]
    public List<Property> Properties { get; set; }
}