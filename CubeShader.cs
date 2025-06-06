using Voxel_Project.OpenGL_Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using System.Linq.Expressions;

namespace Voxel_Project
{
    /// <summary>
    /// Class for rendering several cubes given a CubeShaderBufferSet
    /// </summary>
    internal class CubeShader : ShaderBase
    {
        // Shader binding points
        uint positionsBufferIndex = 0;
        uint scalesBufferIndex = 1;
        uint texturesBufferIndex = 2;

        int sunDepthTextureUnit = 0;
        int moonDepthTextureUnit = 1;

        public CubeShader(string vertPath, string fragPath) : base(vertPath, fragPath)
        {
            this.Use();
            SetInt("sunDepthTexture", sunDepthTextureUnit);
            SetInt("moonDepthTexture", moonDepthTextureUnit);
        }

        /// <summary>
        /// Renders the scene
        /// </summary>
        /// <param name="vertexArray">The vertices of the object to draw</param>
        /// <param name="cubeBuffers">The location, scale, texture, and count information about the object(s)</param>
        /// <param name="isCursor">Whether to draw transparent and ignoring depth</param>
        public void Render(ICamera camera, VertexArray vertexArray, CubeShaderBufferSet cubeBuffers, Scene scene, bool isCursor = false, bool isCloud = false)
        {
            // Bind SSBOs
            cubeBuffers.positions.Use(BufferTarget.ShaderStorageBuffer);
            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, positionsBufferIndex, cubeBuffers.positions);

            cubeBuffers.scales.Use(BufferTarget.ShaderStorageBuffer);
            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, scalesBufferIndex, cubeBuffers.scales);

            cubeBuffers.textures.Use(BufferTarget.ShaderStorageBuffer);
            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, texturesBufferIndex, cubeBuffers.textures);


            // Set shader uniforms
            this.Use();
            SetMat4("view", camera.GetViewMatrix());
            SetMat4("projection", camera.GetProjectionMatrix());
            SetFloat("dayProgress", scene.GetDayProgress());
            SetFloat("dayStrength", scene.DayStrength());
            SetVec3("normSunPosition", scene.GetSunShadowMapper().GetNormPosition());

            SetMat4("sunView", scene.GetSunShadowMapper().GetCamera().GetViewMatrix());
            SetMat4("sunProjection", scene.GetSunShadowMapper().GetCamera().GetProjectionMatrix());

            SetMat4("moonView", scene.GetMoonShadowMapper().GetCamera().GetViewMatrix());
            SetMat4("moonProjection", scene.GetMoonShadowMapper().GetCamera().GetProjectionMatrix());

            SetBool("isCloud", isCloud);
            if (isCursor)
            {
                GL.Disable(EnableCap.DepthTest);
                SetBool("isCursor", true);
            }
            else
            {
                GL.Enable(EnableCap.DepthTest);
                SetBool("isCursor", false);
            }

            scene.GetSunShadowMapper().GetDepthTexture().Use(sunDepthTextureUnit);
            scene.GetMoonShadowMapper().GetDepthTexture().Use(moonDepthTextureUnit);


            vertexArray.Use();

            const int triangleCount = 12;
            const int verticesPerTriangle = 3;
            GL.DrawArraysInstanced(PrimitiveType.Triangles, 0, triangleCount * verticesPerTriangle, cubeBuffers.GetObjectCount());
            GL.Enable(EnableCap.DepthTest);
        }
    }
}
