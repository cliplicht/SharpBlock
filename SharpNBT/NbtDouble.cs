using SharpNBT.Extensions;

namespace SharpNBT
{
    public class NbtDouble : NbtTag
    {
        public double Value { get; set; }
        public override NbtTagType TagType => NbtTagType.Double;

        public NbtDouble(string name, double value) : base(name)
        {
            Value = value;
        }

        public override void WriteTag(Stream stream)
        {
            stream.WriteByte((byte)TagType);
            stream.WriteString(Name);
            WriteData(stream);
        }

        public override void WriteData(Stream stream)
        {
            stream.WriteDouble(Value);
        }
    }
}