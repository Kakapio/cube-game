using System;
using System.Collections.Generic;
using System.Linq;
using Cube_World;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;

namespace Cube_Game
{
    public class Game : GameWindow
    {
        private Camera camera;
        Dictionary<string, ShaderProgram> shaders = new Dictionary<string, ShaderProgram>();
        Chunk chunk = new Chunk();
        
        private Vector2 lastMousePos;
        private const float cubeScale = 1f;
        string activeShader = "default";
        int iboElements;

        public Game (int width, int height, string title) : base(width, height, GraphicsMode.Default, title)
        {
            camera = new Camera(Width, Height);
        }
        
        void Initialize()
        {
            GL.GenBuffers(1, out iboElements);
            
            shaders.Add("default", new ShaderProgram("shader.vert", "shader.frag", true));
            
            chunk.FillUpToY(50, BlockType.Dirt);
            
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
            
            ProcessInput();
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
            
            for (int x = 0; x < chunk.Blocks.GetLength(0); x++) 
            {
                for (int y = 0; y < chunk.Blocks.GetLength(1); y++) 
                {
                    for (int z = 0; z < chunk.Blocks.GetLength(2); z++)
                    {
                        chunk.ModelViewMatrixProjections[x, y, z] =
                            BlockMesh.CalculateModelMatrix(new Vector3(x, y, z), cubeScale) * camera.ViewProjectionMatrix;
                    }
                }
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
                        GL.UniformMatrix4(shaders[activeShader].GetUniform("modelView"), false,
                            ref chunk.ModelViewMatrixProjections[x, y, z]);
                        GL.DrawElements(BeginMode.Triangles, BlockMesh.IndiceCount, DrawElementsType.UnsignedInt, indexAt * sizeof(uint));
                        indexAt += BlockMesh.IndiceCount;
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

        void ProcessInput()
        {
            KeyboardState input = Keyboard.GetState();

            if (input.IsKeyDown(Key.Escape))
                Exit();
            
            if (input.IsKeyDown(Key.W))
            {
                camera.Move(0f, 0.1f, 0f);
            }
            else if (input.IsKeyDown(Key.S))
            {
                camera.Move(0f, -0.1f, 0f);
            }
 
            if (input.IsKeyDown(Key.A))
            {
                camera.Move(-0.1f, 0f, 0f);
            }
            else if (input.IsKeyDown(Key.D))
            {
                camera.Move(0.1f, 0f, 0f);
            }
 
            if (input.IsKeyDown(Key.Q))
            {
                camera.Move(0f, 0f, 0.1f);
            }
            else if (input.IsKeyDown(Key.E))
            {
                camera.Move(0f, 0f, -0.1f);
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
