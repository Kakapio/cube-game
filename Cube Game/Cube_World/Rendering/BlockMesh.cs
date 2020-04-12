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
        public const int TextureCoordsCount = 24;

        /// <summary>
        /// Returns all the vertices of a cube.
        /// </summary>
        /// <returns></returns>
        public static Vector3[] GetAllVertices(Vector3 offset = new Vector3())
        {
            Vector3[] vertices =
            {
                new Vector3(-0.5f, -0.5f,  -0.5f), //Left Bottom Back
                new Vector3(0.5f, -0.5f,  -0.5f), //Right Bottom Back
                new Vector3(0.5f, 0.5f,  -0.5f), //Right Top Back
                new Vector3(-0.5f, 0.5f,  -0.5f), //Left Top Back
                new Vector3(-0.5f, -0.5f,  0.5f), //Left Bottom Front
                new Vector3(0.5f, -0.5f,  0.5f), //Right Bottom Front
                new Vector3(0.5f, 0.5f,  0.5f), //Right Top Front
                new Vector3(-0.5f, 0.5f,  0.5f), //Left Top Front
            };
            
            if (offset != Vector3.Zero)
            {
                for (int i = 0; i < vertices.Length; i++)
                {
                    vertices[i] += offset;
                }
            }

            return vertices;
        }
        
        public static Vector3[] GetCulledVertices(List<Direction> directions, out int usedVertexCount, int offset = 0)
        {
            int[] westVertices = {0, 4, 7, 7, 3, 0};
            int[] southVertices = {1, 0, 3, 3, 2, 1};
            int[] eastVertices = {1, 2, 6, 6, 5, 1};
            int[] northVertices = {4, 5, 6, 6, 7, 4};
            int[] aboveVertices = {6, 2, 3, 3, 7, 6};
            int[] belowVertices = {0, 1, 5, 5, 4, 0};
            
            List<Vector3> culledVertices = new List<Vector3>();

            // //Add all the requested directions to a temporary list.
            // foreach (Direction direction in directions)
            // {
            //     switch (direction)
            //     {
            //         case Direction.Above:
            //             culledVertices.AddRange(aboveVertices);
            //             break;
            //         case Direction.Below:
            //             culledVertices.AddRange(belowVertices);
            //             break;
            //         case Direction.West:
            //             culledVertices.AddRange(westVertices);
            //             break;
            //         case Direction.East:
            //             culledVertices.AddRange(eastVertices);
            //             break;
            //         case Direction.North:
            //             culledVertices.AddRange(northVertices);
            //             break;
            //         case Direction.South:
            //             culledVertices.AddRange(southVertices);
            //             break;
            //     }
            // }
            //
            // if (offset != 0)
            // {
            //     for (int i = 0; i < culledVertices.Count; i++)
            //     {
            //         culledVertices[i] += offset;
            //     }
            // }

            usedVertexCount = directions.Count * 6; //Each face has 6 indices.
            //return culledVertices.ToArray();
            return new Vector3[]
            {
                
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
                //west
                0, 4, 7, 7, 3, 0,
                //south
                1, 0, 3, 3, 2, 1,
                //east
                1, 2, 6, 6, 5, 1,
                //above
                6, 2, 3, 3, 7, 6,
                //north
                4, 5, 6, 6, 7, 4,
                //below
                0, 1, 5, 5, 4, 0
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
        /// <param name="usedFaceCount"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static int[] GetCulledIndices(List<Direction> directions, out int usedFaceCount, int offset = 0)
        {
            int[] westIndices = {0, 4, 7, 7, 3, 0};
            int[] southIndices = {1, 0, 3, 3, 2, 1};
            int[] eastIndices = {1, 2, 6, 6, 5, 1};
            int[] northIndices = {4, 5, 6, 6, 7, 4};
            int[] aboveIndices = {6, 2, 3, 3, 7, 6};
            int[] belowIndices = {0, 1, 5, 5, 4, 0};
            
            List<int> culledIndices = new List<int>();

            //Add all the requested directions to a temporary list.
            foreach (Direction direction in directions)
            {
                switch (direction)
                {
                    case Direction.Above:
                        culledIndices.AddRange(aboveIndices);
                        break;
                    case Direction.Below:
                        culledIndices.AddRange(belowIndices);
                        break;
                    case Direction.West:
                        culledIndices.AddRange(westIndices);
                        break;
                    case Direction.East:
                        culledIndices.AddRange(eastIndices);
                        break;
                    case Direction.North:
                        culledIndices.AddRange(northIndices);
                        break;
                    case Direction.South:
                        culledIndices.AddRange(southIndices);
                        break;
                }
            }
            
            if (offset != 0)
            {
                for (int i = 0; i < culledIndices.Count; i++)
                {
                    culledIndices[i] += offset;
                }
            }

            usedFaceCount = directions.Count * 6; //Each face has 6 indices.
            return culledIndices.ToArray();
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

        public static Vector2[] GetTextureCoords()
        {
            return new Vector2[] 
            {
                // west
                new Vector2(-1.0f, 0.0f),
                new Vector2(0.0f, 0.0f),
                new Vector2(0.0f, 1.0f),
                new Vector2(-1.0f, 1.0f),
 
                // south
                new Vector2(-1.0f, 0.0f),
                new Vector2(0.0f, 0.0f),
                new Vector2(0.0f, 1.0f),
                new Vector2(-1.0f, 1.0f),
 
                // east - only working texcoords
                new Vector2(-1.0f, 0.0f),
                new Vector2(0.0f, 0.0f),
                new Vector2(0.0f, 1.0f),
                new Vector2(-1.0f, 1.0f),
 
                // above
                new Vector2(-1.0f, 0.0f),
                new Vector2(0.0f, 0.0f),
                new Vector2(0.0f, 1.0f),
                new Vector2(-1.0f, 1.0f),
 
                // north
                new Vector2(-1.0f, 0.0f),
                new Vector2(0.0f, 0.0f),
                new Vector2(0.0f, 1.0f),
                new Vector2(-1.0f, 1.0f),
 
                // below
                new Vector2(-1.0f, 0.0f),
                new Vector2(0.0f, 0.0f),
                new Vector2(0.0f, 1.0f),
                new Vector2(-1.0f, 1.0f)
            };
        }
    }
}