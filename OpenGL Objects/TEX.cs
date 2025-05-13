using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;

namespace Voxel_Project.OpenGL_Objects
{
    internal class TEX
    {
        private uint id;

        public void Use(TextureTarget target)
        {
            GL.BindTexture(target, id);
        }

        public static implicit operator uint(TEX tex)
        {
            return tex.id;
        }

        unsafe public TEX()
        {
            fixed (uint* ptr = &id)
                GL.GenTextures(1, ptr);
        }

        unsafe ~TEX()
        {
            fixed (uint* ptr = &id)
                GL.DeleteTextures(1, ptr);
        }
    }
}