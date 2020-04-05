using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;

namespace Cube_Game
{
    public class Game : GameWindow
    {
        private Camera camera;
        private Dictionary<string, ShaderProgram> shaders = new Dictionary<string, ShaderProgram>();
        private Chunk chunk = new Chunk();
        
        private Vector2 lastMousePos;
        private const float cubeScale = 1f;
        private string activeShader = "default";
        private int iboElements;

        public Game (int width, int height, string title) : base(width, height, GraphicsMode.Default, title)
        {
            camera = new Camera(Width, Height);
        }
        
        private void Initialize()
        {
            GL.GenBuffers(1, out iboElements);
            
            shaders.Add("default", new ShaderProgram("shader.vert", "shader.frag", true));
            
            chunk.FillUpToY(1, BlockType.Dirt);
            chunk.SetBlock(new Vector3(5, 0, 8), BlockType.Air);
            chunk.SetBlock(new Vector3(5, 0, 9), BlockType.Air);
            chunk.SetBlock(new Vector3(0, 1, 0), BlockType.Dirt);
            chunk.SetBlock(new Vector3(1, 1, 0), BlockType.Dirt);
            
            lastMousePos = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            CursorVisible = false;
        }
        
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Initialize();
            GL.Enable(EnableCap.DepthTest);
            GL.ClearColor(0.047f, 0.474f, 0.811f, 1.0f);
            GL.PointSize(5f);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            
            ProcessInput((float)e.Time);
            camera.UpdateViewProjectionMatrix();
            
            var (vertData, indiceData, colorData) = BlockMesh.GenerateMeshData(chunk);

            GL.BindBuffer(BufferTarget.ArrayBuffer, shaders[activeShader].GetBuffer("vPosition"));
 
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertData.Length * Vector3.SizeInBytes), vertData, BufferUsageHint.DynamicDraw);
            GL.VertexAttribPointer(shaders[activeShader].GetAttribute("vPosition"), 3, VertexAttribPointerType.Float, false, 0, 0);
 
            if (shaders[activeShader].GetAttribute("vColor") != -1)
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, shaders[activeShader].GetBuffer("vColor"));
                GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(colorData.Length * Vector3.SizeInBytes), colorData, BufferUsageHint.DynamicDraw);
                GL.VertexAttribPointer(shaders[activeShader].GetAttribute("vColor"), 3, VertexAttribPointerType.Float, true, 0, 0);
            }
            
            GL.UseProgram(shaders[activeShader].ProgramID);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, iboElements);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(indiceData.Length * sizeof(int)), indiceData, BufferUsageHint.StaticDraw);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            shaders[activeShader].EnableVertexAttribArrays();

            int indexAt = 0;
            
            for (int x = 0; x < chunk.Blocks.GetLength(0); x++) 
            {
                for (int y = 0; y < chunk.Blocks.GetLength(1); y++) 
                {
                    for (int z = 0; z < chunk.Blocks.GetLength(2); z++)
                    {
                        //Don't render air blocks.
                        if (chunk.Blocks[x, y, z] != (int)BlockType.Air)
                        {
                            //Calculate the projection of the current block accounting for its position.
                            Matrix4 modelViewProjection = BlockMesh.CalculateModelMatrix(new Vector3(x, y, z)) * camera.ViewProjectionMatrix;
                            
                            GL.UniformMatrix4(shaders[activeShader].GetUniform("modelView"), false,
                                ref modelViewProjection);
                            GL.DrawElements(BeginMode.Triangles, BlockMesh.IndiceCount, DrawElementsType.UnsignedInt, indexAt * sizeof(uint));
                            indexAt += BlockMesh.IndiceCount;
                        }
                    }
                }
            }
            
            shaders[activeShader].DisableVertexAttribArrays();

            GL.Flush();
            Context.SwapBuffers();
        }

        protected override void OnFocusedChanged(EventArgs e)
        {
            base.OnFocusedChanged(e);
            
            lastMousePos = new Vector2(Mouse.GetState().X, Mouse.GetState().Y); 
        }

        void ProcessInput(float deltaTime)
        {
            KeyboardState input = Keyboard.GetState();

            if (input.IsKeyDown(Key.Escape))
                Exit();
            
            if (input.IsKeyDown(Key.W))
            {
                camera.Move(new Vector3(0f, 0.1f, 0f) * deltaTime);
            }
            else if (input.IsKeyDown(Key.S))
            {
                camera.Move(new Vector3(0f, -0.1f, 0f) * deltaTime);
            }
 
            if (input.IsKeyDown(Key.A))
            {
                camera.Move(new Vector3(-0.1f, 0f, 0f) * deltaTime);
            }
            else if (input.IsKeyDown(Key.D))
            {
                camera.Move(new Vector3(0.1f, 0f, 0f) * deltaTime);
            }
 
            if (input.IsKeyDown(Key.Space))
            {
                camera.Move(new Vector3(0f, 0f, 0.1f) * deltaTime);
            }
            else if (input.IsKeyDown(Key.ShiftLeft))
            {
                camera.Move(new Vector3(0f, 0f, -0.1f) * deltaTime);
            }

            if (Focused)
            {
                Vector2 delta = lastMousePos - new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
                lastMousePos += delta;

                camera.AddRotation(delta.X, delta.Y);
                lastMousePos = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            }
        }
    }
}
