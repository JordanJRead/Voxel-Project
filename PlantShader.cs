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
    internal class PlantShader : ShaderBase
    {
        uint positionsBufferIndex = 0;
        uint growthsBufferIndex = 1;
        uint texturesBufferIndex = 2;

        public PlantShader(string vertPath, string fragPath) : base(vertPath, fragPath)
        {
        }

        /// <summary>
        /// Renders the scene
        /// </summary>
        /// <param name="vertexArray">The vertices of the object to draw</param>
        /// <param name="buffers">The location, texture, and count information about the object(s)</param>
        public void Render(Camera camera, VertexArray vertexArray, PlantShaderBufferSet buffers)
        {
            // Bind SSBOs
            buffers.positions.Use(BufferTarget.ShaderStorageBuffer);
            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, positionsBufferIndex, buffers.positions);

            buffers.growths.Use(BufferTarget.ShaderStorageBuffer);
            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, growthsBufferIndex, buffers.growths);

            buffers.textures.Use(BufferTarget.ShaderStorageBuffer);
            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, texturesBufferIndex, buffers.textures);

            this.Use();
            SetMat4("view", camera.GetViewMatrix());
            SetMat4("projection", camera.GetProjectionMatrix());

            vertexArray.Use();

            const int triangleCount = 6;
            const int verticesPerTriangle = 3;
            GL.DrawArraysInstanced(PrimitiveType.Triangles, 0, triangleCount * verticesPerTriangle, buffers.GetObjectCount());
        }
    }
}
