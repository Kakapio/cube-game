using System;
using System.Diagnostics;
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
        /// Fill a chunk with a type of block, bottom to top.
        /// </summary>
        /// <param name="yStop"></param>
        /// <param name="block"></param>
        public void FillUpToY(int yStop, BlockType block)
        {
            for (int x = 0; x < Blocks.GetLength(0); x++)
            {
                for (int y = 0; y < yStop; y++)
                {
                    for (int z = 0; z < Blocks.GetLength(2); z++)
                    {
                        Blocks[x, y, z] = (int)block;
                    }
                }
            }
        }

        /// <summary>
        /// Set the block at a given coordinate.
        /// </summary>
        /// <param name="coordinate"></param>
        public void SetBlock(Vector3 coordinate, BlockType block)
        {
            if (VerifyCoordinate(coordinate))
            {
                Blocks[(int) coordinate.X, (int) coordinate.Y, (int) coordinate.Z] = (int)block;
            }
        }

        /// <summary>
        /// Returns false if a coordinate is out of the bounds of the chunk.
        /// </summary>
        /// <param name="coordinate"></param>
        public bool VerifyCoordinate(Vector3 coordinate)
        {
            if (coordinate.X < 0 || coordinate.X > ChunkWidth - 1)
                return false;
            if (coordinate.Y < 0 || coordinate.Y > ChunkHeight - 1)
                return false;
            if (coordinate.Z < 0 || coordinate.Z > ChunkDepth - 1)
                return false;
            else
                return true;
        }
    }
}