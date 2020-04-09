using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using OpenTK;

namespace Cube_Game
{
    public class Chunk
    {
        public int ChunkWidth { get; }
        public int ChunkHeight { get; }
        public int ChunkDepth { get; }
        public int UsedVerticeCount { get; private set; }
        public int UsedIndiceCount { get; private set; }
        public bool HasChanged { get; set; } //used to indicate whether the chunk has been modified since last mesh data was sent out.
        
        public int[, ,] Blocks { get; }

        public Chunk(int width = 16, int height = 128, int depth = 16)
        {
            ChunkWidth = width;
            ChunkHeight = height;
            ChunkDepth = depth;
            HasChanged = true;
            Blocks = new int[ChunkWidth, ChunkHeight, ChunkDepth];

            //Set every block to Air to start with.
            for (int x = 0; x < ChunkWidth; x++)
            {
                for (int y = 0; y < ChunkHeight; y++)
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

            HasChanged = true;
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
                HasChanged = true;
            }
        }
        
        /// <summary>
        /// Return the block at a given coordinate
        /// </summary>
        /// <param name="coordinate"></param>
        /// <param name="block"></param>
        public int GetBlock(Vector3 coordinate)
        {
            if (VerifyCoordinate(coordinate))
            {
                return Blocks[(int) coordinate.X, (int) coordinate.Y, (int) coordinate.Z];
            }
            else
                throw new InvalidInputException("Coordinates were invalid!");
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
            if (VerifyCoordinate(coordinate) == false)
                throw new InvalidInputException("Coordinates were invalid!");
            if (GetBlock(coordinate) == (int)BlockType.Air)
                throw new InvalidInputException("Block at given coordinate is an air block!");
            
            List<Direction> directions = new List<Direction>();
            Vector3 above, below, west, east, north, south;
            above = below = west = east = north = south = coordinate;
            above.Y += 1;
            below.Y -= 1;
            west.X -= 1;
            east.X += 1;
            north.Z += 1;
            south.Z -= 1;
            
            if (VerifyCoordinate(above) && Blocks[(int)above.X, (int)above.Y, (int)above.Z] == 0)
            {
                directions.Add(Direction.Above);
            }
            if (VerifyCoordinate(below) && Blocks[(int)below.X, (int)below.Y, (int)below.Z] == 0)
            {
                directions.Add(Direction.Below);
            }
            if (VerifyCoordinate(west) && Blocks[(int)west.X, (int)west.Y, (int)west.Z] == 0)
            {
                directions.Add(Direction.West);
            }
            if (VerifyCoordinate(east) && Blocks[(int)east.X, (int)east.Y, (int)east.Z] == 0)
            {
                directions.Add(Direction.East);
            }
            if (VerifyCoordinate(north) && Blocks[(int)north.X, (int)north.Y, (int)north.Z] == 0)
            {
                directions.Add(Direction.North);
            }
            if (VerifyCoordinate(south) && Blocks[(int)south.X, (int)south.Y, (int)south.Z] == 0)
            {
                directions.Add(Direction.South);
            }
            
            // Handling corner blocks. Their exposed faces should still be rendered.
            if ((int)coordinate.Y == 0 && !directions.Contains(Direction.Below))
                directions.Add(Direction.Below);
            if ((int)coordinate.X == 0 && !directions.Contains(Direction.West))
                directions.Add(Direction.West);
            if ((int)coordinate.X == ChunkWidth - 1 && !directions.Contains(Direction.East))
                directions.Add(Direction.East);
            if ((int)coordinate.Z == ChunkDepth - 1 && !directions.Contains(Direction.North))
                directions.Add(Direction.North);
            if ((int)coordinate.Z == 0 && !directions.Contains(Direction.South))
                directions.Add(Direction.South);
            
            return directions;
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
                            int usedFaceCount = 0;
                            
                            vertices.AddRange(BlockMesh.GetAllVertices(new Vector3(x, y, z)).ToList());
                            //Add the indices while maintaining proper order via an offset.
                            indices.AddRange(BlockMesh.GetCulledIndices(SidesExposedToAir(new Vector3(x, y, z)), out usedFaceCount, vertCount).ToList());
                            
                            colors.AddRange(BlockMesh.GetColorData().ToList());
        
                            vertCount += BlockMesh.VertCount;
                            indiceCount += usedFaceCount * 6;
                        }
                    }
                }
            }

            UsedVerticeCount = vertCount;
            UsedIndiceCount = indiceCount;
            HasChanged = false; //method was called, meaning the new data has been sent back out.
            
            return (vertices.ToArray(), indices.ToArray(), colors.ToArray());
        }
    }
}