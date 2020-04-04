using System;
using Cube_World;

namespace Cube_Game
{
    public class Chunk
    {
        public int[, ,] blocks = new int[16, 128, 16];

        public Chunk()
        {
            //Set every block to Air to start with.
            for (int x = 0; x < blocks.GetLength(0); x++) //Iterate through all X-values
            {
                for (int y = 0; y < blocks.GetLength(1); y++) //Iterate through each Y-value connected to every X-value
                {
                    for (int z = 0; z < blocks.GetLength(2); z++)
                    {
                        blocks[x, y, z] = (int)BlockType.Air;
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
            for (int x = 0; x < blocks.GetLength(0); x++)
            {
                for (int y = 0; y <= yStop; y++)
                {
                    for (int z = 0; z < blocks.GetLength(2); z++)
                    {
                        blocks[x, y, z] = (int)blockType;
                    }
                }
            }
        }
    }
}