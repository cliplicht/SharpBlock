using System.Text.Json;
using SharpNBT.Extensions;

namespace SharpBlock.Pakets;

public class StatusResponsePaket : OutgoingPaket
{
    public override int PacketId => 0x00;

    public string Name { get; set; }
    public int Protocol { get; set; }
    public int Max { get; set; }
    public int Online { get; set; }
    public object[] Sample { get; set; }
    public string Text { get; set; }
    public string Favicon { get; set; }
    public bool EnforceSecureChat { get; set; }

    public override void Write(Stream stream)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        var response = JsonSerializer.Serialize(new
        {
            version = new
            {
                name = Name,
                protocol = Protocol
            },
            players = new
            {
                max = Max,
                online = Online,
                sample = Sample
            },
            description = new
            {
                text = Text
            },
            favicon = Favicon,
            enforcesSecureChat = EnforceSecureChat
        }, options);
        
        stream.WriteStringMc(response);
    }
}