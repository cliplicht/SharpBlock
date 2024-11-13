using SharpNBT.Extensions;

namespace SharpNBT
{
    public class NbtFloat : NbtTag
    {
        public float Value { get; set; }
        public override NbtTagType TagType => NbtTagType.Float;

        public NbtFloat(string name, float value) : base(name)
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
            stream.WriteFloat(Value);
        }
    }
}