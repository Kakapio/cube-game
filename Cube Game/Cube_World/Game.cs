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
        Camera camera = new Camera();
        Dictionary<string, ShaderProgram> shaders = new Dictionary<string, ShaderProgram>();
        List<Block> blocks = new List<Block>();
        
        private Vector2 lastMousePos;
        private const float cubeScale = 0.25f;
        string activeShader = "default";
        int iboElements;

        public Game (int width, int height, string title) : base(width, height, GraphicsMode.Default, title)
        {
        }
        
        void Initialize()
        {
            GL.GenBuffers(1, out iboElements);
            
            shaders.Add("default", new ShaderProgram("shader.vert", "shader.frag", true));
            
            blocks.Add(new Block());
            blocks.Add(new Block());
            
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
            
            List<Vector3> vertices = new List<Vector3>();
            List<int> indices = new List<int>();
            List<Vector3> colors = new List<Vector3>();

            int vertCount = 0;

            //Here we build a mesh before handing it off.
            foreach (Block block in blocks)
            {
                vertices.AddRange(block.GetVertices().ToList());
                indices.AddRange(block.GetIndices(vertCount).ToList());
                colors.AddRange(block.GetColorData().ToList());
                vertCount += block.VertCount;
            }

            Vector3[] vertData = vertices.ToArray();
            int[] indiceData = indices.ToArray();
            Vector3[] colorData = colors.ToArray();

            GL.BindBuffer(BufferTarget.ArrayBuffer, shaders[activeShader].GetBuffer("vPosition"));
 
            GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, (IntPtr)(vertData.Length * Vector3.SizeInBytes), vertData, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(shaders[activeShader].GetAttribute("vPosition"), 3, VertexAttribPointerType.Float, false, 0, 0);
 
            if (shaders[activeShader].GetAttribute("vColor") != -1)
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, shaders[activeShader].GetBuffer("vColor"));
                GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, (IntPtr)(colorData.Length * Vector3.SizeInBytes), colorData, BufferUsageHint.StaticDraw);
                GL.VertexAttribPointer(shaders[activeShader].GetAttribute("vColor"), 3, VertexAttribPointerType.Float, true, 0, 0);
            }
            
            blocks[0].Position = new Vector3(-1.25f, -0.25f, -2.0f);
            blocks[0].Scale = Vector3.One * cubeScale;
            
            blocks[1].Position = new Vector3(-1f, -0.25f, -2.0f);
            blocks[1].Scale = Vector3.One * cubeScale;

            for (int i = 0; i < blocks.Count; i++) //TODO delete this not necessary
            {
                blocks[i].Scale = Vector3.One * cubeScale;

                Vector3 newPos = new Vector3(-2f, -0.25f, -2.0f);
                newPos.X += i * cubeScale;
                blocks[i].Position = newPos;
            }
            
            foreach (Block block in blocks)
            {
                block.CalculateModelMatrix();
                block.ViewProjectionMatrix = camera.GetViewMatrix() * Matrix4.CreatePerspectiveFieldOfView(1.3f, 
                    Width / (float) Height, 1.0f, 40.0f);
                block.ModelViewProjectionMatrix = block.ModelMatrix * block.ViewProjectionMatrix;
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
            foreach (Block block in blocks)
            {
                GL.UniformMatrix4(shaders[activeShader].GetUniform("modelView"), false, ref block.ModelViewProjectionMatrix);
                GL.DrawElements(BeginMode.Triangles, block.IndiceCount, DrawElementsType.UnsignedInt, indexAt * sizeof(uint));
                indexAt += block.IndiceCount;
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
            
            if (input.IsKeyDown(Key.F))
            {
                blocks.Add(new Block());
            }
            
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
