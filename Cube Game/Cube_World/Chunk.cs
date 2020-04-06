using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
            for (int x = 0; x < ChunkWidth; x++) //Iterate through all X-values
            {
                for (int y = 0; y < ChunkHeight; y++) //Iterate through each Y-value connected to every X-value
                {
                    for (int z = 0; z < ChunkDepth; z++)
                    {
                        Blocks[x, y, z] = (int)BlockType.Air;
                    }
                }
            }
        }

        /// <summary>
        /// Fill a chunk with a number of layers of a type of block, bottom to top.
        /// </summary>
        /// <param name="yStop"></param>
        /// <param name="block"></param>
        public void FillUpToY(int yStop, BlockType block)
        {
            for (int x = 0; x < ChunkWidth; x++)
            {
                for (int y = 0; y < yStop; y++)
                {
                    for (int z = 0; z < ChunkDepth; z++)
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
                Blocks[(int)coordinate.X, (int)coordinate.Y, (int)coordinate.Z] = (int)block;
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
        
        /// <summary>
        /// Return the directions around a block that are exposed to air.
        /// </summary>
        /// <returns></returns>
        public List<Direction> SidesExposedToAir(Vector3 coordinate)
        {
            List<Direction> directions = new List<Direction>();
            Vector3 above, below, left, right, front, behind;
            above = below = left = right = front = behind = coordinate;
            above.Y += 1;
            below.Y -= 1;
            left.X -= 1;
            right.X += 1;
            front.Z += 1;
            behind.Z -= 1;
            
            if (VerifyCoordinate(coordinate))
            {
                if (VerifyCoordinate(above) && Blocks[(int)above.X, (int)above.Y, (int)above.Z] == 0)
                {
                    directions.Add(Direction.Above);
                }
                if (VerifyCoordinate(below) && Blocks[(int)below.X, (int)below.Y, (int)below.Z] == 0)
                {
                    directions.Add(Direction.Below);
                }
                if (VerifyCoordinate(left) && Blocks[(int)left.X, (int)left.Y, (int)left.Z] == 0)
                {
                    directions.Add(Direction.Left);
                }
                if (VerifyCoordinate(right) && Blocks[(int)right.X, (int)right.Y, (int)right.Z] == 0)
                {
                    directions.Add(Direction.Right);
                }
                if (VerifyCoordinate(front) && Blocks[(int)front.X, (int)front.Y, (int)front.Z] == 0)
                {
                    directions.Add(Direction.Front);
                }
                if (VerifyCoordinate(behind) && Blocks[(int)behind.X, (int)behind.Y, (int)behind.Z] == 0)
                {
                    directions.Add(Direction.Behind);
                }
                
                //Handling corner blocks. Their exposed faces should still be rendered.
                //Note: Do not try to render corner blocks of the y-axis.
                if ((int)coordinate.X == 0 && !directions.Contains(Direction.Left))
                    directions.Add(Direction.Front);
                if ((int)coordinate.X == 0 && !directions.Contains(Direction.Right))
                    directions.Add(Direction.Behind);
                if ((int)coordinate.Y == 0 && !directions.Contains(Direction.Below))
                    directions.Add(Direction.Below);
                if ((int)coordinate.Z == 0 && !directions.Contains(Direction.Behind))
                    directions.Add(Direction.Left);
                if ((int)coordinate.Z == ChunkDepth - 1 && !directions.Contains(Direction.Front))
                    directions.Add(Direction.Right);
            }

            return directions;
        }

        /// <summary>
        /// Returns the number of vertices in the current chunk.
        /// </summary>
        /// <returns></returns>
        public int GetVerticeCount()
        {
            int nonAirBlocks = 0;
            
            for (int x = 0; x < ChunkWidth; x++)
            {
                for (int y = 0; y < ChunkHeight; y++)
                {
                    for (int z = 0; z < ChunkDepth; z++)
                    {
                        if (Blocks[x, y, z] != (int)BlockType.Air)
                            nonAirBlocks += BlockMesh.IndiceCount; //8 * 4, vertices per face
                    }
                }
            }

            return nonAirBlocks;
        }
        
        public (Vector3[] vertices, int[] indices, Vector3[] colors) GenerateMeshData()
        {
            List<Vector3> vertices = new List<Vector3>();
            List<int> indices = new List<int>();
            List<Vector3> colors = new List<Vector3>();
        
            int vertCount = 0;
            int indiceCount = 0;
        
            for (int x = 0; x < ChunkWidth; x++)
            {
                for (int y = 0; y < ChunkHeight; y++)
                {
                    for (int z = 0; z < ChunkDepth; z++)
                    {
                        //Don't hand back mesh data for air blocks.
                        if (Blocks[x, y, z] != (int) BlockType.Air)
                        {
                            int indCount = 0;
                            
                            vertices.AddRange(BlockMesh.GetAllVertices(new Vector3(x, y, z)).ToList());
                            //Add the indices while maintaining proper order via an offset.
                            indices.AddRange(BlockMesh.GetCulledIndices(SidesExposedToAir(new Vector3(x, y, z)), out indCount, vertCount).ToList());
                            colors.AddRange(BlockMesh.GetColorData().ToList());
        
                            vertCount += BlockMesh.VertCount;
                            indiceCount += indCount;
                        }
                    }
                }
            }
        
            return (vertices.ToArray(), indices.ToArray(), colors.ToArray());
        }
    }
}