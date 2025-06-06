using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StbImageSharp;
using OpenTK.Graphics.OpenGL4;
using Voxel_Project.OpenGL_Objects;

namespace Voxel_Project
{
    /// <summary>
    /// Loads and manages the plant texxtures
    /// </summary>
    internal class PlantTextxureManager
    {
        Texture2D[] textures = new Texture2D[Enum.GetNames<Plant.Type>().Length];
        long[] textureHandles = new long[Enum.GetNames<Plant.Type>().Length]; // Should be the same size as the voxel types

        public PlantTextxureManager()
        {
            for (int i = 0; i < textureHandles.Length; i++)
            {
                textures[i] = new Texture2D($"Images/Plants/{(Plant.Type)i}.png", "Images/Cubes/none.png");
                textureHandles[i] = GL.Arb.GetTextureHandle(textures[i]);
                GL.Arb.MakeTextureHandleResident(textureHandles[i]);
            }
        }

        /// <summary>
        /// Used when using a shader that utilizes bindless textures
        /// </summary>
        public ulong GetBindlessTextureHandle(Plant.Type plantType)
        {
            return (ulong)textureHandles[(int)plantType];
        }
    }
}
