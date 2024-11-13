using SharpNBT.Extensions;

namespace SharpNBT
{
    public class NbtByteArray : NbtTag
    {
        public List<byte> Value { get; set; }
        public override NbtTagType TagType => NbtTagType.ByteArray;

        public NbtByteArray(string name, List<byte> value) : base(name)
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
            stream.WriteInt(Value.Count);
            foreach (var b in Value)
            {
                stream.WriteByte(b);
            }
        }
    }
}