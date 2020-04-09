using System;
using OpenTK;

namespace Cube_Game
{
    public class World
    {
        public Chunk[,] Chunks { get; }

        public World(int xSize, int ySize)
        {
            Chunks = new Chunk[xSize, ySize];

            //Temporary way to fill up chunks with data.
            for (int i = 0; i < Chunks.GetLength(0); i++)
            {
                for (int j = 0; j < Chunks.GetLength(1); j++)
                {
                    Random random = new Random();
                    
                    Chunks[i, j] = new Chunk();
                    Chunks[i, j].FillUpToY(random.Next(5, 15), BlockType.Dirt);
                }
            }
        }
    }
}