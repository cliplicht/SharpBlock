using System.Text.Json;
using System.Text.Json.Serialization;

namespace SharpBlock.Core.Utils;

public class FlexibleGuidConverter : JsonConverter<Guid>
{
    public override Guid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var stringValue = reader.GetString();

        if (stringValue is { Length: 32 })
        {
            stringValue = stringValue.Insert(8, "-").Insert(13, "-").Insert(18, "-").Insert(23, "-");
        }
        
        return Guid.Parse(stringValue);
    }

    public override void Write(Utf8JsonWriter writer, Guid value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}