using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;

namespace Voxel_Project.OpenGL_Objects
{
    internal class FBO
    {
        private uint id;

        public void Use()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, id);
        }

        unsafe public FBO()
        {
            fixed (uint* ptr = &id)
                GL.GenFramebuffers(1, ptr);
        }

        public static implicit operator uint(FBO f)
        {
            return f.id;
        }

        unsafe ~FBO()
        {
            fixed (uint* ptr = &id)
                GL.DeleteFramebuffers(1, ptr);
        }
    }
}
