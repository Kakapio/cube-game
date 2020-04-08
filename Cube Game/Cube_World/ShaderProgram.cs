using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OpenTK.Graphics.OpenGL4;

namespace Cube_Game
{
    public class ShaderProgram
    {
        public int ProgramID = -1;
        public int VShaderID = -1;
        public int FShaderID = -1;
        public int AttributeCount = 0;
        public int UniformCount = 0;
 
        private Dictionary<String, AttributeInfo> attributes = new Dictionary<string, AttributeInfo>();
        private Dictionary<String, UniformInfo> uniforms = new Dictionary<string, UniformInfo>();
        private Dictionary<String, uint> buffers = new Dictionary<string, uint>();
        
        public ShaderProgram()
        {
            ProgramID = GL.CreateProgram();
        }
        
        public ShaderProgram(String vshader, String fshader, bool fromFile = false)
        {
            ProgramID = GL.CreateProgram();
 
            if (fromFile)
            {
                LoadShaderFromFile(vshader, ShaderType.VertexShader);
                LoadShaderFromFile(fshader, ShaderType.FragmentShader);
            }
            else
            {
                LoadShaderFromString(vshader, ShaderType.VertexShader);
                LoadShaderFromString(fshader, ShaderType.FragmentShader);
            }
 
            Link();
            GenBuffers();
        }
        
        private void LoadShader(String code, ShaderType type, out int address)
        {
            address = GL.CreateShader(type);
            GL.ShaderSource(address, code);
            GL.CompileShader(address);
            GL.AttachShader(ProgramID, address);
            Console.WriteLine(GL.GetShaderInfoLog(address));
        }
        
        public void LoadShaderFromString(String code, ShaderType type)
        {
            if (type == ShaderType.VertexShader)
            {
                LoadShader(code, type, out VShaderID);
            }
            else if (type == ShaderType.FragmentShader)
            {
                LoadShader(code, type, out FShaderID);
            }
        }
 
        public void LoadShaderFromFile(String filename, ShaderType type)
        {
            using (StreamReader sr = new StreamReader(filename))
            {
                if (type == ShaderType.VertexShader)
                {
                    LoadShader(sr.ReadToEnd(), type, out VShaderID);
                }
                else if (type == ShaderType.FragmentShader)
                {
                    LoadShader(sr.ReadToEnd(), type, out FShaderID);
                }
            }
        }
        
        public void Link()
        {
            GL.LinkProgram(ProgramID);
 
            Console.WriteLine(GL.GetProgramInfoLog(ProgramID));
 
            GL.GetProgram(ProgramID, GetProgramParameterName.ActiveAttributes, out AttributeCount);
            GL.GetProgram(ProgramID, GetProgramParameterName.ActiveUniforms, out UniformCount);
 
            for (int i = 0; i < AttributeCount; i++)
            {
                AttributeInfo info = new AttributeInfo();
                int length = 0;
 
                string name= "";
 
                GL.GetActiveAttrib(ProgramID, i, 256, out length, out info.size, out info.type, out name);
 
                info.name = name;
                info.address = GL.GetAttribLocation(ProgramID, info.name);
                attributes.Add(name, info);
            }
 
            for (int i = 0; i < UniformCount; i++)
            {
                UniformInfo info = new UniformInfo();
                int length = 0;
 
                string name= "";

                GL.GetActiveUniform(ProgramID, i, 256, out length, out info.size, out info.type, out name);
                
                info.name = name;
                uniforms.Add(name, info);
                info.address = GL.GetUniformLocation(ProgramID, info.name);
            }
        }
        
        public void GenBuffers()
        {
            for (int i = 0; i < attributes.Count; i++)
            {
                uint buffer = 0;
                GL.GenBuffers(1, out buffer);
 
                buffers.Add(attributes.Values.ElementAt(i).name, buffer);
            }
 
            for (int i = 0; i < uniforms.Count; i++)
            {
                uint buffer = 0;
                GL.GenBuffers(1, out buffer);
 
                buffers.Add(uniforms.Values.ElementAt(i).name, buffer);
            }
        }
        
        public void EnableVertexAttribArrays()
        {
            for (int i = 0; i < attributes.Count; i++)
            {
                GL.EnableVertexAttribArray(attributes.Values.ElementAt(i).address);
            }
        }
 
        public void DisableVertexAttribArrays()
        {
            for (int i = 0; i < attributes.Count; i++)
            {
                GL.DisableVertexAttribArray(attributes.Values.ElementAt(i).address);
            }
        }
        
        public int GetAttribute(string name)
        {
            if (attributes.ContainsKey(name))
            {
                return attributes[name].address;
            }
            else
                return -1;
        }
 
        public int GetUniform(string name)
        {
            if (uniforms.ContainsKey(name))
            {
                return uniforms[name].address;
            }
            else
                return -1;
        }
 
        public uint GetBuffer(string name)
        {
            if (buffers.ContainsKey(name))
            {
                return buffers[name];
            }
            else
                return 0;
        }
    }
    
    public class AttributeInfo
    {
        public String name = "";
        public int address = -1;
        public int size = 0;
        public ActiveAttribType type;
    }
 
    public class UniformInfo
    {
        public String name = "";
        public int address = -1;
        public int size = 0;
        public ActiveUniformType type;
    }
}