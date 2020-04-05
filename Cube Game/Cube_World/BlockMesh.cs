using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK;

namespace Cube_Game
{
    public static class BlockMesh
    {
        public const int VertCount = 8;
        public const int IndiceCount = 36;
        public const int ColorDataCount = 8;

        /// <summary>
        /// Returns all the vertices of a cube.
        /// </summary>
        /// <returns></returns>
        public static Vector3[] GetAllVertices()
        {
            return new Vector3[] 
            {
                new Vector3(-0.5f, -0.5f,  -0.5f), //Left Bottom Back
                new Vector3(0.5f, -0.5f,  -0.5f), //Right Bottom Back
                new Vector3(0.5f, 0.5f,  -0.5f), //Right Top Back
                new Vector3(-0.5f, 0.5f,  -0.5f), //Left Top Back - One face is completed!
                new Vector3(-0.5f, -0.5f,  0.5f), //Left Bottom Front
                new Vector3(0.5f, -0.5f,  0.5f), //Right Bottom Front
                new Vector3(0.5f, 0.5f,  0.5f), //Right Top Front
                new Vector3(-0.5f, 0.5f,  0.5f), //Left Top Front
            };
        }
        
        /// <summary>
        /// Returns all the indices of a cube.
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static int[] GetAllIndices(int offset = 0)
        {
            int[] indices = 
            {
                //left
                0, 2, 1,
                0, 3, 2,
                //back
                1, 2, 6,
                6, 5, 1,
                //right
                4, 5, 6,
                6, 7, 4,
                //top
                2, 3, 6,
                6, 3, 7,
                //front
                0, 7, 3,
                0, 4, 7,
                //bottom
                0, 1, 5,
                0, 5, 4
            };
            
            if (offset != 0)
            {
                for (int i = 0; i < indices.Length; i++)
                {
                    indices[i] += offset;
                }
            }
            
            return indices;
        }

        /// <summary>
        /// Generate a cube's indices given the sides that are to be rendered.
        /// </summary>
        /// <param name="directions"></param>
        /// <param name="usedVerticeCount"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static int[] GetCulledIndices(List<Direction> directions, out int usedVerticeCount, int offset = 0)
        {
            int[] leftIndices = {0, 2, 1, 0, 3, 2};
            int[] behindIndices = {1, 2, 6, 6, 5, 1};
            int[] rightIndices = {4, 5, 6, 6, 7, 4};
            int[] aboveIndices = {2, 3, 6, 6, 3, 7};
            int[] frontIndices = {0, 7, 3, 0, 4, 7};
            int[] belowIndices = {0, 1, 5, 0, 5, 4};

            List<int> finalArray = new List<int>();

            //Add all the requested directions to a temporary list.
            foreach (Direction direction in directions)
            {
                switch (direction)
                {
                    case Direction.Left:
                        finalArray.AddRange(leftIndices);
                        break;
                    case Direction.Behind:
                        finalArray.AddRange(behindIndices);
                        break;
                    case Direction.Right:
                        finalArray.AddRange(rightIndices);
                        break;
                    case Direction.Above:
                        finalArray.AddRange(aboveIndices);
                        break;
                    case Direction.Front:
                        finalArray.AddRange(frontIndices);
                        break;
                    case Direction.Below:
                        finalArray.AddRange(belowIndices);
                        break;
                }
            }

            usedVerticeCount = directions.Count * 6;
            return finalArray.ToArray();
        }

        /// <summary>
        /// Return the directions around a block that are exposed to air.
        /// </summary>
        /// <returns></returns>
        public static Direction[] SidesExposedToAir(Vector3 coordinate, Chunk chunk)
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
            
            if (chunk.VerifyCoordinate(coordinate))
            {
                if (chunk.VerifyCoordinate(above) && chunk.Blocks[(int)above.X, (int)above.Y, (int)above.Z] == 0)
                    directions.Add(Direction.Above);
                if (chunk.VerifyCoordinate(below) && chunk.Blocks[(int)below.X, (int)below.Y, (int)below.Z] == 0)
                    directions.Add(Direction.Below);
                if (chunk.VerifyCoordinate(left) && chunk.Blocks[(int)left.X, (int)left.Y, (int)left.Z] == 0)
                    directions.Add(Direction.Left);
                if (chunk.VerifyCoordinate(right) && chunk.Blocks[(int)right.X, (int)right.Y, (int)right.Z] == 0)
                    directions.Add(Direction.Right);
                if (chunk.VerifyCoordinate(front) && chunk.Blocks[(int)front.X, (int)front.Y, (int)front.Z] == 0)
                    directions.Add(Direction.Front);
                if (chunk.VerifyCoordinate(behind) && chunk.Blocks[(int)behind.X, (int)behind.Y, (int)behind.Z] == 0)
                    directions.Add(Direction.Behind);
            }

            return directions.ToArray();
        }
        
        public static Vector3[] GetColorData()
        {
            return new Vector3[] 
            {
                new Vector3( 1f, 0f, 0f),
                new Vector3( 0f, 0f, 1f),
                new Vector3( 0f, 1f, 0f),
                new Vector3( 1f, 0f, 0f),
                new Vector3( 0f, 0f, 1f),
                new Vector3( 0f, 1f, 0f),
                new Vector3( 1f, 0f, 0f),
                new Vector3( 0f, 0f, 1f)
            };
        }

        public static Matrix4 CalculateModelMatrix(Vector3 position, float scale = 1f)
        {
            return Matrix4.CreateScale(scale) * Matrix4.CreateRotationX(0) * Matrix4.CreateRotationY(0) * 
                   Matrix4.CreateRotationZ(0) * Matrix4.CreateTranslation(position);
        }
        
        public static (Vector3[] vertices, int[] indices, Vector3[] colors) GenerateMeshData(Chunk chunk)
        {
            List<Vector3> vertices = new List<Vector3>();
            List<int> indices = new List<int>();
            List<Vector3> colors = new List<Vector3>();

            int vertCount = 0;

            for (int x = 0; x < chunk.Blocks.GetLength(0); x++)
            {
                for (int y = 0; y < chunk.Blocks.GetLength(1); y++)
                {
                    for (int z = 0; z < chunk.Blocks.GetLength(2); z++)
                    {
                        //Don't hand back mesh data for air blocks.
                        if (chunk.Blocks[x, y, z] != (int) BlockType.Air)
                        {
                            vertices.AddRange(GetAllVertices().ToList());
                            //Add the indices while maintaining proper order via an offset.
                            indices.AddRange(GetAllIndices(vertCount).ToList());
                            colors.AddRange(GetColorData().ToList());

                            vertCount += VertCount;
                        }
                    }
                }
            }

            return (vertices.ToArray(), indices.ToArray(), colors.ToArray());
        }
    }
}