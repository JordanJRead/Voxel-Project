﻿using StbImageSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Voxel_Project.OpenGL_Objects;
using OpenTK.Graphics.OpenGL4;

namespace Voxel_Project
{
    internal class Texture2D
    {
        TEX tex = new TEX();

        /// <param name="path">'Images/NAME.EXT'</param>
        public Texture2D(string path, string defaultPath = "Images/Cubes/none.png")
        {
            string projectPath = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName;
            string imagePath = projectPath + '/' + path;
            string defaultImagePath = projectPath + '/' + defaultPath;

            tex.Use(TextureTarget.Texture2D);
            StbImage.stbi_set_flip_vertically_on_load(1);

            ImageResult image;
            try
            {
                image = ImageResult.FromStream(File.OpenRead(imagePath), ColorComponents.RedGreenBlueAlpha);
            }
            catch (Exception ex)
            {
                image = ImageResult.FromStream(File.OpenRead(defaultImagePath), ColorComponents.RedGreenBlueAlpha);
            }

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.NearestMipmapNearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Nearest);
        }

        public static implicit operator uint(Texture2D texture)
        {
            return texture.tex;
        }

        public void Use(int textureUnit)
        {
            GL.ActiveTexture(TextureUnit.Texture0 + textureUnit);
            tex.Use(TextureTarget.Texture2D);
        }
    }
}
