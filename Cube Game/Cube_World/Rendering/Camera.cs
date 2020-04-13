using System;
using OpenTK;

namespace Cube_Game
{
    public class Camera
    {
        public Vector3 Position = new Vector3(0, 0, 0);
        public Vector3 Orientation = new Vector3((float)Math.PI, 0f, 0f);
        public Matrix4 ViewProjectionMatrix;
        public float MoveSpeed = 0.1f;
        public float MouseSensitivity = 0.0025f;

        private int screenWidth, screenHeight;

        public Camera(int Width, int Height)
        {
            screenWidth = Width;
            screenHeight = Height;
            
            ViewProjectionMatrix = GetViewMatrix() * Matrix4.CreatePerspectiveFieldOfView(1.3f,
                screenWidth / (float) screenHeight, 1.0f, 40.0f);
        }

        public Matrix4 GetViewMatrix()
        {
            Vector3 lookAt = new Vector3();
            
            lookAt.X = (float)(Math.Sin(Orientation.X) * Math.Cos(Orientation.Y));
            lookAt.Y = (float)Math.Sin(Orientation.Y);
            lookAt.Z = (float)(Math.Cos(Orientation.X) * Math.Cos(Orientation.Y));

            return Matrix4.LookAt(Position, Position + lookAt, Vector3.UnitY);
        }

        public void Move(Vector3 amount)
        {
            Vector3 offset = new Vector3();
            Vector3 forward = new Vector3((float)Math.Sin(Orientation.X), 0, (float)(Math.Cos(Orientation.X)));
            Vector3 right = new Vector3(-forward.Z, 0, forward.X);

            offset += amount.X * right;
            offset += amount.Y * forward;
            offset.Y += amount.Z;
            
            offset.NormalizeFast();
            offset = Vector3.Multiply(offset, MoveSpeed);

            Position += offset;
        }

        public void AddRotation(float x, float y)
        {
            x = x * MouseSensitivity;
            y = y * MouseSensitivity;

            Orientation.X = (Orientation.X + x) % ((float) Math.PI * 2);
            Orientation.Y = Math.Max(Math.Min(Orientation.Y + y, (float)Math.PI / 2.0f - 0.1f), (float)-Math.PI / 2.0f + 0.1f);
        }

        public void UpdateViewProjectionMatrix()
        {
            ViewProjectionMatrix = GetViewMatrix() * Matrix4.CreatePerspectiveFieldOfView(1.3f,
                screenWidth / (float)screenHeight, 0.1f, 100.0f);
        }
    }
}