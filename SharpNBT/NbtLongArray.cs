using SharpNBT.Extensions;
using System.Collections.Generic;

namespace SharpNBT
{
    public class NbtLongArray : NbtTag
    {
        public List<long> Value { get; set; }
        public override NbtTagType TagType => NbtTagType.LongArray;

        public NbtLongArray(string name, List<long> value) : base(name)
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
            // Write the length of the array
            stream.WriteInt(Value.Count);

            // Write each long value
            foreach (var item in Value)
            {
                stream.WriteLong(item);
            }
        }
    }
}