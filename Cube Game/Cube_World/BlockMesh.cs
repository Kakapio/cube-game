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
        public static Vector3[] GetAllVertices(Vector3 offset = new Vector3())
        {
            Vector3[] vertices =
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
            
            if (offset != Vector3.Zero)
            {
                for (int i = 0; i < vertices.Length; i++)
                {
                    vertices[i] += offset;
                }
            }

            return vertices;
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
        /// <param name="usedIndiceCount"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static int[] GetCulledIndices(List<Direction> directions, out int usedIndiceCount, int offset = 0)
        {
            int[] leftIndices = {0, 2, 1, 0, 3, 2};
            int[] behindIndices = {1, 2, 6, 6, 5, 1};
            int[] rightIndices = {4, 5, 6, 6, 7, 4};
            int[] aboveIndices = {2, 3, 6, 6, 3, 7};
            int[] frontIndices = {0, 7, 3, 0, 4, 7};
            int[] belowIndices = {0, 1, 5, 0, 5, 4};

            List<int> culledIndices = new List<int>();

            //Add all the requested directions to a temporary list.
            foreach (Direction direction in directions)
            {
                switch (direction)
                {
                    case Direction.Left:
                        culledIndices.AddRange(leftIndices);
                        break;
                    case Direction.Behind:
                        culledIndices.AddRange(behindIndices);
                        break;
                    case Direction.Right:
                        culledIndices.AddRange(rightIndices);
                        break;
                    case Direction.Above:
                        culledIndices.AddRange(aboveIndices);
                        break;
                    case Direction.Front:
                        culledIndices.AddRange(frontIndices);
                        break;
                    case Direction.Below:
                        culledIndices.AddRange(belowIndices);
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

            usedIndiceCount = directions.Count * 6; //Each face has 6 indices.
            return culledIndices.ToArray();
        }
        
        // public static int[] GetCulledVertices (List<Direction> directions, out int usedIndiceCount, int offset = 0)
        // {
        //     int[] leftVertices = {0, 2, 1, 0, 3, 2};
        //     int[] behindVertices = {1, 2, 6, 6, 5, 1};
        //     int[] rightVertices = {4, 5, 6, 6, 7, 4};
        //     int[] aboveVertices = {2, 3, 6, 6, 3, 7};
        //     int[] frontVertices = {0, 7, 3, 0, 4, 7};
        //     int[] belowVertices = {0, 1, 5, 0, 5, 4};
        // }

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
    }
}