using SharpNBT.Extensions;

namespace SharpNBT
{
    public class NbtLong : NbtTag
    {
        public long Value { get; set; }
        public override NbtTagType TagType => NbtTagType.Long;

        public NbtLong(string name, long value) : base(name)
        {
            Value = value;
        }

        public override void WriteTag(Stream stream)
        {
            // Write tag type and name
            stream.WriteByte((byte)TagType);
            stream.WriteString(Name);

            // Write data
            WriteData(stream);
        }

        public override void WriteData(Stream stream)
        {
            stream.WriteLong(Value);
        }
    }
}