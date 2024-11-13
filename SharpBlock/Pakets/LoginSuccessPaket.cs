using SharpNBT.Extensions;

namespace SharpBlock.Pakets;

public class LoginSuccessPacket : OutgoingPaket
{
    public override int PacketId => 0x02;

    public string Id { get; set; } = string.Empty; // UUID ohne Bindestriche
    public string Username { get; set; } = string.Empty;
    public List<PlayerProperty> Properties { get; set; } = new List<PlayerProperty>();

    public override void Write(Stream stream)
    {
        stream.WriteString(Id);       // UUID ohne Bindestriche
        stream.WriteString(Username); // Spielername

        // Für Protokollversion 768 sind die Eigenschaften erforderlich
        stream.WriteVarInt(Properties.Count);
        foreach (var property in Properties)
        {
            stream.WriteString(property.Name);
            stream.WriteString(property.Value);
            if (!string.IsNullOrEmpty(property.Signature))
            {
                stream.WriteBoolean(true);
                stream.WriteString(property.Signature);
            }
            else
            {
                stream.WriteBoolean(false);
            }
        }
    }
}