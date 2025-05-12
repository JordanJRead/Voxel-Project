using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;

namespace Voxel_Project.OpenGL_Objects
{
    internal class VAO
    {
        private uint id;

        public void Use()
        {
            GL.BindVertexArray(id);
        }

        public static implicit operator uint(VAO va)
        {
            return va.id;
        }

        unsafe public VAO()
        {
            fixed (uint* ptr = &id)
                GL.GenVertexArrays(1, ptr);
        }

        unsafe ~VAO()
        {
            fixed (uint* ptr = &id)
                GL.DeleteVertexArrays(1, ptr);
        }
    }
}
