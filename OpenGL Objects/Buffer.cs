using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;

namespace OpenTK_Test.OpenGL_Objects
{
    internal class Buffer
    {
        private uint id;

        public void Use(BufferTarget target)
        {
            GL.BindBuffer(target, id);
        }

        unsafe public Buffer()
        {
            fixed (uint* ptr = &id)
                GL.GenBuffers(1, ptr);
        }

        unsafe ~Buffer()
        {
            fixed (uint* ptr = &id)
                GL.DeleteBuffers(1, ptr);
        }
    }
}
