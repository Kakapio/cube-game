using System;
using Cube_World;
using OpenTK;

namespace Cube_Game
{
    public class Chunk
    {
        public int ChunkWidth { get; }
        public int ChunkHeight { get; }
        public int ChunkDepth { get; }
        
        public int[, ,] Blocks { get; }
        public Matrix4[, ,] ModelViewMatrixProjections;

        public Chunk(int width = 16, int height = 128, int depth = 16)
        {
            ChunkWidth = width;
            ChunkHeight = height;
            ChunkDepth = depth;
            Blocks = new int[ChunkWidth, ChunkHeight, ChunkDepth];
            ModelViewMatrixProjections = new Matrix4[ChunkWidth, ChunkHeight, ChunkDepth];
            
            //Set every block to Air to start with.
            for (int x = 0; x < Blocks.GetLength(0); x++) //Iterate through all X-values
            {
                for (int y = 0; y < Blocks.GetLength(1); y++) //Iterate through each Y-value connected to every X-value
                {
                    for (int z = 0; z < Blocks.GetLength(2); z++)
                    {
                        Blocks[x, y, z] = (int)BlockType.Air;
                    }
                }
            }
        }

        /// <summary>
        /// Fill up a chunk with a block up to a certain y-value, bottom to top.
        /// </summary>
        /// <param name="yStop"></param>
        /// <param name="blockType"></param>
        public void FillUpToY(int yStop, BlockType blockType)
        {
            for (int x = 0; x < Blocks.GetLength(0); x++)
            {
                for (int y = 0; y <= yStop; y++)
                {
                    for (int z = 0; z < Blocks.GetLength(2); z++)
                    {
                        Blocks[x, y, z] = (int)blockType;
                    }
                }
            }
        }
    }
}