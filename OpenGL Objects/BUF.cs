using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;

namespace Voxel_Project.OpenGL_Objects
{
    internal class BUF
    {
        private uint id;

        public void Use(BufferTarget target)
        {
            GL.BindBuffer(target, id);
        }

        unsafe public BUF()
        {
            fixed (uint* ptr = &id)
                GL.GenBuffers(1, ptr);
        }

        public static implicit operator uint(BUF b)
        {
            return b.id;
        }

        unsafe ~BUF()
        {
            fixed (uint* ptr = &id)
                GL.DeleteBuffers(1, ptr);
        }
    }
}
