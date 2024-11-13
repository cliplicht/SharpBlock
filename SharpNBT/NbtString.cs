using SharpNBT.Extensions;

namespace SharpNBT
{
    public class NbtString : NbtTag
    {
        public string Value { get; set; }
        public override NbtTagType TagType => NbtTagType.String;

        public NbtString(string name, string value) : base(name)
        {
            Value = value;
        }

        public override void WriteTag(Stream stream)
        {
            // Write tag type and name
            stream.WriteByte((byte)TagType);
            stream.WriteString(Name);

            // Write the string value
            WriteData(stream);
        }

        public override void WriteData(Stream stream)
        {
            stream.WriteString(Value);
        }
    }
}