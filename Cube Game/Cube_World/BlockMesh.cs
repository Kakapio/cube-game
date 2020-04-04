using System;
using System.Collections.Generic;
using System.Linq;
using Cube_Game;
using OpenTK;

namespace Cube_World
{
    public static class BlockMesh
    {
        public const int VertCount = 8;
        public const int IndiceCount = 36;
        public const int ColorDataCount = 8;
        
        public static Vector3[] GetVertices()
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

        public static int[] GetCulledIndices(List<Direction> directions, int offset = 0)
        {
            return new int[] 
                {1, 2};
        }
        
        //TODO add a proper indice filtering method that returns a properly culled set of indices for a mesh
        
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
        
        public static Matrix4 CalculateModelMatrix(Vector3 position)
        {
            //0s are in place of rotation coordinates, x, y, z in order of appearance.
            return Matrix4.CreateScale(Vector3.One) * Matrix4.CreateRotationX(0) * Matrix4.CreateRotationY(0) * 
                   Matrix4.CreateRotationZ(0) * Matrix4.CreateTranslation(position);
        }
        
        public static Matrix4 CalculateModelMatrix(Vector3 position, float scale)
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
                        vertices.AddRange(BlockMesh.GetVertices().ToList());
                        //Add the indices while maintaining proper order via an offset.
                        indices.AddRange(BlockMesh.GetAllIndices(vertCount).ToList());
                        colors.AddRange(BlockMesh.GetColorData().ToList());

                        vertCount += BlockMesh.VertCount;
                    }
                }
            }

            return (vertices.ToArray(), indices.ToArray(), colors.ToArray());
        }
    }
}