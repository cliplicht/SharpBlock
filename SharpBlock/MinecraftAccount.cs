using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;

namespace SharpBlock;

public class MinecraftAccount
{
    public string Name { get; set; } = string.Empty;
    public List<PlayerProperty> Properties { get; set; } =  new List<PlayerProperty>();
    public List<object> ProfileActions { get; set; } = new List<object>();
    public Guid Id { get; set; }

    public MinecraftAccount()
    {
        
    }

    public MinecraftAccount(Guid id, string playerName)
    {
        Id = id;
        Name = playerName;
    }
}

public class PlayerProperty
{
    public string Name { get; set; }
    public string Value { get; set; }
    public string? Signature { get; set; }
}