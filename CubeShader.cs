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

        public CubeShader(string vertPath, string fragPath) : base(vertPath, fragPath)
        {
            SetInt("white", 0);
        }

        /// <summary>
        /// Renders the scene
        /// </summary>
        /// <param name="vertexArray">The vertices of the object to draw</param>
        /// <param name="buffers">The location, texture, and count information about the object(s)</param>
        /// <param name="drawCursor">Whether to draw transparent and ignoring depth</param>
        public void Render(Camera camera, VertexArray vertexArray, ShaderBufferSet buffers, TextureManager textureManager, bool drawCursor = false)
        {
            // Bind SSBOs
            buffers.positions.Use(BufferTarget.ShaderStorageBuffer);
            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, positionsBufferIndex, buffers.positions);

            buffers.scales.Use(BufferTarget.ShaderStorageBuffer);
            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, scalesBufferIndex, buffers.scales);

            buffers.textures.Use(BufferTarget.ShaderStorageBuffer);
            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, texturesBufferIndex, buffers.textures);

            this.Use();
            SetMat4("view", camera.GetViewMatrix());
            SetMat4("projection", camera.GetProjectionMatrix());

            GL.ActiveTexture(TextureUnit.Texture0);
            textureManager.whiteNoise.Use();

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
            GL.DrawArraysInstanced(PrimitiveType.Triangles, 0, triangleCount * verticesPerTriangle, buffers.GetObjectCount());
        }
    }
}
