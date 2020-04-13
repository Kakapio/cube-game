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
                new Vector3(-0.5f, -0.5f,  -0.5f), //Left Bottom Back 0
                new Vector3(0.5f, -0.5f,  -0.5f), //Right Bottom Back 1
                new Vector3(0.5f, 0.5f,  -0.5f), //Right Top Back 2
                new Vector3(-0.5f, 0.5f,  -0.5f), //Left Top Back 3
                new Vector3(-0.5f, -0.5f,  0.5f), //Left Bottom Front 4
                new Vector3(0.5f, -0.5f,  0.5f), //Right Bottom Front 5
                new Vector3(0.5f, 0.5f,  0.5f), //Right Top Front 6
                new Vector3(-0.5f, 0.5f,  0.5f), //Left Top Front 7
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
        
        public static Vector3[] GetCulledVertices(List<Direction> directions, out int usedVertexCount, Vector3 offset = new Vector3())
        {
            Vector3[] westVertices =
            {
                new Vector3(-0.5f, -0.5f,  -0.5f), //Left Bottom Back 0
                new Vector3(-0.5f, -0.5f,  0.5f), //Left Bottom Front 4
                new Vector3(-0.5f, 0.5f,  0.5f), //Left Top Front 7
                new Vector3(-0.5f, 0.5f,  -0.5f) //Left Top Back 3
            };
            Vector3[] southVertices =
            {
                new Vector3(-0.5f, -0.5f,  -0.5f), //Left Bottom Back 0
                new Vector3(0.5f, -0.5f,  -0.5f), //Right Bottom Back 1
                new Vector3(0.5f, 0.5f,  -0.5f), //Right Top Back 2
                new Vector3(-0.5f, 0.5f,  -0.5f) //Left Top Back 3
            };
            Vector3[] eastVertices =
            {
                new Vector3(0.5f, -0.5f,  0.5f), //Right Bottom Front 5
                new Vector3(0.5f, -0.5f,  -0.5f), //Right Bottom Back 1
                new Vector3(0.5f, 0.5f,  -0.5f), //Right Top Back 2
                new Vector3(0.5f, 0.5f,  0.5f) //Right Top Front 6
            };
            Vector3[] northVertices =
            {
                new Vector3(-0.5f, -0.5f,  0.5f), //Left Bottom Front 4
                new Vector3(0.5f, -0.5f,  0.5f), //Right Bottom Front 5
                new Vector3(0.5f, 0.5f,  0.5f), //Right Top Front 6
                new Vector3(-0.5f, 0.5f,  0.5f) //Left Top Front 7
            };
            Vector3[] aboveVertices =
            {
                new Vector3(-0.5f, 0.5f,  0.5f), //Left Top Front 7
                new Vector3(0.5f, 0.5f,  0.5f), //Right Top Front 6
                new Vector3(0.5f, 0.5f,  -0.5f), //Right Top Back 2
                new Vector3(-0.5f, 0.5f,  -0.5f) //Left Top Back 3
            };
            Vector3[] belowVertices =
            {
                new Vector3(-0.5f, -0.5f,  -0.5f), //Left Bottom Back 0
                new Vector3(0.5f, -0.5f,  -0.5f), //Right Bottom Back 1
                new Vector3(0.5f, -0.5f,  0.5f), //Right Bottom Front 5
                new Vector3(-0.5f, -0.5f,  0.5f) //Left Bottom Front 4
            };
            
            List<Vector3> culledVertices = new List<Vector3>();

            //Add all the requested directions to a temporary list.
            foreach (Direction direction in directions)
            {
                switch (direction)
                {
                    case Direction.Above:
                        culledVertices.AddRange(aboveVertices);
                        break;
                    case Direction.Below:
                        culledVertices.AddRange(belowVertices);
                        break;
                    case Direction.West:
                        culledVertices.AddRange(westVertices);
                        break;
                    case Direction.East:
                        culledVertices.AddRange(eastVertices);
                        break;
                    case Direction.North:
                        culledVertices.AddRange(northVertices);
                        break;
                    case Direction.South:
                        culledVertices.AddRange(southVertices);
                        break;
                }
            }
            
            if (offset != Vector3.Zero)
            {
                for (int i = 0; i < culledVertices.Count; i++)
                {
                    culledVertices[i] += offset;
                }
            }
            
            usedVertexCount = directions.Count * 4;
            return culledVertices.ToArray();
        }
        
        public static int[] GetCulledIndices(List<Direction> directions, int offset = 0)
        {
            List<int> culledIndices = new List<int>();
            int[] indices = {0, 1, 2, 2, 3, 0};

            int facesAdded = 0; //Number of faces we've added to the list so far.
            
            foreach (Direction direction in directions)
            {
                if (direction == Direction.South)
                    culledIndices.AddRange(OffsetData(indices, offset + facesAdded).Reverse());
                else
                    culledIndices.AddRange(OffsetData(indices, offset + facesAdded));
                
                facesAdded += 4; //Offset by the number of vertices in each face.
            }

            return culledIndices.ToArray();
        }

        public static int[] OffsetData(int[] data, int offset)
        {
            //Add the data to our new array to be passed back.
            int[] modifiedData = new int[data.Length];
            for (int i = 0; i < data.Length; i++)
                modifiedData[i] = data[i];
            
            if (offset != 0)
            {
                for (int i = 0; i < data.Length; i++)
                {
                    modifiedData[i] += offset;
                }
            }

            return modifiedData.ToArray();
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