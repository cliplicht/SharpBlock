namespace SharpBlock.World
{
    public class Chunk
    {
        public int X { get; }
        public int Z { get; }
        public byte[,] Blocks { get; }

        public Chunk(int x, int z)
        {
            X = x;
            Z = z;
            Blocks = new byte[16, 256]; // 16x16 Blöcke in der X- und Z-Achse, 256 in der Y-Achse

            // Einfaches Flachwelt-Generierung: Alles bis Y=64 ist Grasblock (Block-ID: 2)
            for (x = 0; x < 16; x++)
            {
                for (z = 0; z < 16; z++)
                {
                    for (int y = 0; y <= 64; y++)
                    {
                        Blocks[x, y] = 2; // Grasblock
                    }
                }
            }
        }
    }
}