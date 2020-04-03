using OpenTK;

namespace Cube_Game
{
    public class Block
    {
        public Vector3 Position = Vector3.Zero;
        public Vector3 Rotation = Vector3.Zero;
        public Vector3 Scale = Vector3.One;

        public int VertCount = 8;
        public int IndiceCount = 36;
        public int ColorDataCount = 8;
        public Matrix4 ModelMatrix = Matrix4.Identity;
        public Matrix4 ViewProjectionMatrix = Matrix4.Identity;
        public Matrix4 ModelViewProjectionMatrix = Matrix4.Identity;
        
        public Block()
        {
            VertCount = 8;
            IndiceCount = 36;
            ColorDataCount = 8;
        }
        
        public Block(Vector3 position, Vector3 rotation, Vector3 scale)
        {
            VertCount = 8;
            IndiceCount = 36;
            ColorDataCount = 8;

            Position = position;
            Rotation = rotation;
            Scale = scale;
        }

        public Vector3[] GetVertices()
        {
            return new Vector3[] 
            {
                new Vector3(-0.5f, -0.5f,  -0.5f),
                new Vector3(0.5f, -0.5f,  -0.5f),
                new Vector3(0.5f, 0.5f,  -0.5f),
                new Vector3(-0.5f, 0.5f,  -0.5f),
                new Vector3(-0.5f, -0.5f,  0.5f),
                new Vector3(0.5f, -0.5f,  0.5f),
                new Vector3(0.5f, 0.5f,  0.5f),
                new Vector3(-0.5f, 0.5f,  0.5f),
            };
        }

        public int[] GetIndices(int offset = 0)
        {
            int[] indices = new int[] 
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

        public Vector3[] GetColorData()
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

        public void CalculateModelMatrix()
        {
            ModelMatrix = Matrix4.CreateScale(Scale) * Matrix4.CreateRotationX(Rotation.X) * Matrix4.CreateRotationY(Rotation.Y) * 
                          Matrix4.CreateRotationZ(Rotation.Z) * Matrix4.CreateTranslation(Position);
        }
    }
}