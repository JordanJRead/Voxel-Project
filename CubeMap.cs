using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Voxel_Project.OpenGL_Objects;
using OpenTK.Graphics.OpenGL4;
using StbImageSharp;

namespace Voxel_Project
{
    internal class CubeMap
    {
        TEX tex = new TEX();
        public CubeMap(string path, string defaultPath = "Images/Cubes/none.png")
        {
            string projectPath = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName;
            string imagePath = projectPath + '/' + path;
            string defaultImagePath = projectPath + '/' + defaultPath;

            tex.Use(TextureTarget.TextureCubeMap);
            StbImageSharp.StbImage.stbi_set_flip_vertically_on_load(1);

            ImageResult image;
            try
            {
                image = ImageResult.FromStream(File.OpenRead(imagePath), ColorComponents.RedGreenBlueAlpha);
            }
            catch (Exception ex)
            {
                image = ImageResult.FromStream(File.OpenRead(defaultImagePath), ColorComponents.RedGreenBlueAlpha);
            }

            // Each cube face
            for (int i = 0; i < 6; ++i)
            {
                GL.TexImage2D(TextureTarget.TextureCubeMapPositiveX + i, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);
            }

            GL.GenerateMipmap(GenerateMipmapTarget.TextureCubeMap);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.NearestMipmapNearest);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Nearest);
        }

        public static implicit operator uint(CubeMap cubeMap)
        {
            return cubeMap.tex;
        }

        public void Use()
        {
            GL.BindTexture(TextureTarget.TextureCubeMap, tex);
        }
    }
}
