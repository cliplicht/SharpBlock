using SharpNBT.Extensions;
using System.Collections;

namespace SharpNBT
{
    public class NbtCompound : NbtTag, IEnumerable<NbtTag>
    {
        public List<NbtTag> Tags { get; } = new List<NbtTag>();
        public override NbtTagType TagType => NbtTagType.Compound;

        public NbtCompound(string name) : base(name)
        {
        }

        public void Add(NbtTag tag)
        {
            Tags.Add(tag);
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
            // Write child tags
            foreach (var tag in Tags)
            {
                tag.WriteTag(stream);
            }

            // Write TAG_End
            stream.WriteByte((byte)NbtTagType.End);
        }

        public IEnumerator<NbtTag> GetEnumerator()
        {
            return Tags.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Tags.GetEnumerator();
        }
    }
}