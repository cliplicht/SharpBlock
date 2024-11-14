using System.Text.Json.Serialization;

namespace SharpBlock.Core.Models;

public class Property
{
    [JsonPropertyName("name")] public string Name { get; set; }
    [JsonPropertyName("value")] public string Value { get; set; }
    [JsonPropertyName("signature")] public string Signature { get; set; }

    public bool IsSigned => !string.IsNullOrEmpty(Signature);
}