using SharpNBT.Extensions;

namespace SharpNBT
{
    public class NbtByte : NbtTag
    {
        public byte Value { get; set; }
        public override NbtTagType TagType => NbtTagType.Byte;

        public NbtByte(string name, byte value) : base(name)
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
            stream.WriteByte(Value);
        }
    }
}