using SharpNBT.Extensions;

namespace SharpNBT
{
    public class NbtInt : NbtTag
    {
        public int Value { get; set; }
        public override NbtTagType TagType => NbtTagType.Int;

        public NbtInt(string name, int value) : base(name)
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
            stream.WriteInt(Value);
        }
    }
}