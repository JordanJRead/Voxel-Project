using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;

namespace OpenTK_Test.OpenGL_Objects
{
    internal class VertexArray
    {
        private uint id;

        public void Use()
        {
            GL.BindVertexArray(id);
        }

        unsafe public VertexArray()
        {
            fixed (uint* ptr = &id)
                GL.GenVertexArrays(1, ptr);
        }

        unsafe ~VertexArray()
        {
            fixed (uint* ptr = &id)
                GL.DeleteVertexArrays(1, ptr);
        }
    }
}
