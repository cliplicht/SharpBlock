using SharpNBT.Extensions;
using System.Collections;

namespace SharpNBT
{
    public class NbtList : NbtTag, IEnumerable<NbtTag>
    {
        public List<NbtTag> Tags { get; } = new List<NbtTag>();
        public NbtTagType ElementType { get; }
        public override NbtTagType TagType => NbtTagType.List;

        public NbtList(string name, NbtTagType elementType) : base(name)
        {
            ElementType = elementType;
        }

        public void Add(NbtTag tag)
        {
            if (tag.TagType != ElementType)
                throw new InvalidOperationException($"All elements in the list must be of type {ElementType}");
            Tags.Add(tag);
        }

        public override void WriteTag(Stream stream)
        {
            // Write tag type and name
            stream.WriteByte((byte)TagType);
            stream.WriteString(Name);

            WriteData(stream);
        }

        public override void WriteData(Stream stream)
        {
            // Write element type
            stream.WriteByte((byte)ElementType);

            // Write length
            stream.WriteInt(Tags.Count);

            // Write elements
            foreach (var tag in Tags)
            {
                tag.WriteData(stream);
            }
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