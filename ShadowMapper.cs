﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Voxel_Project.OpenGL_Objects;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Voxel_Project
{
    /// <summary>
    /// Manages a shadow (depth map) from a celestial object
    /// </summary>
    internal class ShadowMapper
    {
        FBO fbo = new FBO();
        Texture2D depthTexture;
        CelestialCamera camera = new CelestialCamera();
        static int imageSize = 4096 * 4;
        bool isSun;
        Vector3 normPosition; // Position of sun at a distance of 1 away

        public ShadowMapper(int screenWidth, int screenHeight, bool isSun = true)
        {
            this.isSun = isSun;
            fbo.Use();

            depthTexture = new Texture2D(imageSize, imageSize);

            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2D, depthTexture, 0);
            GL.DrawBuffer(DrawBufferMode.None);
            GL.ReadBuffer(ReadBufferMode.None);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public Vector3 GetNormPosition()
        {
            return normPosition;
        }

        public void UpdateShadows(Scene scene, int screenWidth, int screenHeight)
        {
            normPosition = new Vector3(
                MathF.Cos(2 * MathF.PI * scene.GetDayProgress()),
                MathF.Sin(2 * MathF.PI * scene.GetDayProgress()),
                MathF.Sin(2 * MathF.PI * scene.GetDayProgress()) * 0.2f
                ).Normalized();

            Vector3 sunPosition = normPosition * 100;
            Vector3 moonPosition = -sunPosition;
            
            if (isSun)
                camera.SetPosition(sunPosition);
            else
                camera.SetPosition(moonPosition);

            fbo.Use();
            GL.Viewport(0, 0, imageSize, imageSize);
            GL.Clear(ClearBufferMask.DepthBufferBit);

            /*
             README
            Only the sun is rendering to the depth texture, yet the moon's 
             */
            //if (isSun)
                scene.RenderDepth(camera);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.Viewport(0, 0, screenWidth, screenHeight);
        }

        public Texture2D GetDepthTexture()
        {
            return depthTexture;
        }

        public CelestialCamera GetCamera()
        {
            return camera;
        }
    }
}
