using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Voxel_Project.OpenGL_Objects;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Voxel_Project
{
    internal class ShadowMapper
    {
        FBO fbo = new FBO();
        Texture2D depthTexture;
        OrthoCamera camera = new OrthoCamera();

        public ShadowMapper(int screenWidth, int screenHeight)
        {
            fbo.Use();

            depthTexture = new Texture2D(screenWidth, screenHeight);

            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2D, depthTexture, 0);
            GL.DrawBuffer(DrawBufferMode.None);
            GL.ReadBuffer(ReadBufferMode.None);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public void UpdateShadows(Scene scene)
        {
            camera.SetPosition(new Vector3(MathF.Cos(2 * MathF.PI * scene.GetDayProgress()), MathF.Sin(2 * MathF.PI * scene.GetDayProgress()), 0) * 10);
            fbo.Use();
            GL.Clear(ClearBufferMask.DepthBufferBit);

            scene.RenderDepth(camera);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public Texture2D GetDepthTexture()
        {
            return depthTexture;
        }
    }
}
