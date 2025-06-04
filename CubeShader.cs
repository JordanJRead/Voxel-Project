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
    /// Class for rendering several of a specific type of object
    /// </summary>
    internal class CubeShader : ShaderBase
    {
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
        /// <param name="cubeBuffers">The location, texture, and count information about the object(s)</param>
        /// <param name="drawCursor">Whether to draw transparent and ignoring depth</param>
        public void Render(ICamera camera, VertexArray vertexArray, CubeShaderBufferSet cubeBuffers, float dayProgress, ShadowMapper sunShadowMapper, ShadowMapper moonShadowMapper, bool drawCursor = false, bool isCloud = false)
        {
            // Bind SSBOs
            cubeBuffers.positions.Use(BufferTarget.ShaderStorageBuffer);
            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, positionsBufferIndex, cubeBuffers.positions);

            cubeBuffers.scales.Use(BufferTarget.ShaderStorageBuffer);
            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, scalesBufferIndex, cubeBuffers.scales);

            cubeBuffers.textures.Use(BufferTarget.ShaderStorageBuffer);
            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, texturesBufferIndex, cubeBuffers.textures);

            this.Use();
            SetMat4("view", camera.GetViewMatrix());
            SetMat4("projection", camera.GetProjectionMatrix());
            SetFloat("dayProgress", dayProgress);

            SetMat4("sunView", sunShadowMapper.GetCamera().GetViewMatrix());
            SetMat4("sunProjection", sunShadowMapper.GetCamera().GetProjectionMatrix());

            SetMat4("moonView", moonShadowMapper.GetCamera().GetViewMatrix());
            SetMat4("moonProjection", moonShadowMapper.GetCamera().GetProjectionMatrix());

            sunShadowMapper.GetDepthTexture().Use(sunDepthTextureUnit);
            moonShadowMapper.GetDepthTexture().Use(moonDepthTextureUnit);

            SetBool("isCloud", isCloud);
            if (drawCursor)
            {
                GL.Disable(EnableCap.DepthTest);
                SetBool("isCursor", true);
            }
            else
            {
                GL.Enable(EnableCap.DepthTest);
                SetBool("isCursor", false);
            }

            vertexArray.Use();

            const int triangleCount = 12;
            const int verticesPerTriangle = 3;
            GL.DrawArraysInstanced(PrimitiveType.Triangles, 0, triangleCount * verticesPerTriangle, cubeBuffers.GetObjectCount());
            GL.Enable(EnableCap.DepthTest);
        }
    }
}
