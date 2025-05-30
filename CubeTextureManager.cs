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
    internal class CubeTextureManager
    {
        CubeMap[] cubeMaps = new CubeMap[(int)Voxel.Type.none + 1];
        long[] textureHandles = new long[(int)Voxel.Type.none + 1]; // Should be the same size as the voxel types

        public CubeTextureManager()
        {
            for (int i = 0; i < textureHandles.Length; i++)
            {
                cubeMaps[i] = new CubeMap($"Images/Cubes/{((Voxel.Type)i)}.png", "Images/Cubes/none.png");
                textureHandles[i] = GL.Arb.GetTextureHandle(cubeMaps[i]);
                GL.Arb.MakeTextureHandleResident(textureHandles[i]);
            }
        }

        /// <summary>
        /// Used when using a shader that utilizes bindless textures
        /// </summary>
        public ulong GetBindlessTextureHandle(Voxel.Type voxelType)
        {
            return (ulong)textureHandles[(int)voxelType];
        }
    }
}
