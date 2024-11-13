namespace SharpNBT
{
    public abstract class NbtTag
    {
        public string Name { get; set; }
        public abstract NbtTagType TagType { get; }

        protected NbtTag(string name)
        {
            Name = name;
        }

        public abstract void WriteTag(Stream stream);

        public abstract void WriteData(Stream stream);

        public static NbtTag ReadTag(Stream stream)
        {
            // Implement reading logic if needed
            throw new NotImplementedException();
        }
    }
}