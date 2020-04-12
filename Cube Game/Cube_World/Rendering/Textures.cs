using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using OpenTK.Graphics.OpenGL4;

namespace Cube_Game
{
    public static class Textures
    {
        public const int NumberTexturesInAtlas = 1;
        
        private static Dictionary<string, int> textures = new Dictionary<string, int>();
        private static string mainFolderDirectory;

        static Textures()
        {
            mainFolderDirectory = Directory.GetCurrentDirectory();
            mainFolderDirectory = Path.GetFullPath(Path.Combine(mainFolderDirectory, @"..\..\"));
            
            LoadImage(Path.Combine(mainFolderDirectory, "Textures\\Grass_Side.png"), "grass_side");
            LoadImage(Path.Combine(mainFolderDirectory, "Textures\\Grass_Top.png"), "grass_top");
            LoadImage(Path.Combine(mainFolderDirectory, "Textures\\Grass_Bottom.png"), "grass_bottom");
        }
        
        /// <summary>
        /// Loads the image at the given address into the GPU, generates mipmaps, and stores the texture in a dictionary via name.
        /// </summary>
        /// <param name="fileAddress"></param>
        /// <param name="textureName"></param>
        private static void LoadImage(string fileAddress, string textureName)
        {
            Bitmap file = new Bitmap(fileAddress);
            LoadImage(file, textureName);
        }
        
        /// <summary>
        /// Loads the given image into the GPU, generates mipmaps, and stores the texture in a dictionary via name.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="textureName"></param>
        private static void LoadImage(Bitmap image, string textureName)
        {
            int texID = GL.GenTexture();
            textures.Add(textureName, texID);
 
            GL.BindTexture(TextureTarget.Texture2D, texID);
            BitmapData data = image.LockBits(new Rectangle(0, 0, image.Width, image.Height),
                ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
 
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
                OpenTK.Graphics.OpenGL4.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
 
            image.UnlockBits(data);
            
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
 
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        }

        public static int GetTexture(string name)
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