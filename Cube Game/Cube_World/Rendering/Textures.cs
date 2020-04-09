using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using OpenTK.Graphics.OpenGL4;

namespace Cube_Game
{
    public class Textures
    {
        private Dictionary<string, int> textures = new Dictionary<string, int>();

        public Textures()
        {
        }
        
        /// <summary>
        /// Loads the image at the given address into the GPU, generates mipmaps, and stores the texture in a dictionary via name.
        /// </summary>
        /// <param name="fileAddress"></param>
        /// <param name="name"></param>
        private void LoadImage(string fileAddress, string name)
        {
            Bitmap file = new Bitmap(fileAddress);
            LoadImage(file, name);
        }
        
        /// <summary>
        /// Loads the given image into the GPU, generates mipmaps, and stores the texture in a dictionary via name.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="name"></param>
        private void LoadImage(Bitmap image, string name)
        {
            int texID = GL.GenTexture();
            textures.Add(name, texID);
 
            GL.BindTexture(TextureTarget.Texture2D, texID);
            BitmapData data = image.LockBits(new Rectangle(0, 0, image.Width, image.Height),
                ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
 
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
                OpenTK.Graphics.OpenGL4.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
 
            image.UnlockBits(data);
 
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
 
        }

        public int GetTexture(string name)
        {
            if (textures.ContainsKey(name))
                return textures[name];
            else
            {
                throw new InvalidInputException("Inputted name does not exist in currently stored textures!");
            }
        }
    }
}