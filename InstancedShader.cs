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
    internal class InstancedShader : ShaderBase
    {
        uint positionsBufferIndex = 0;
        uint scalesBufferIndex = 1;
        uint texturesBufferIndex = 2;

        public InstancedShader(string vertPath, string fragPath) : base(vertPath, fragPath)
        {
        }

        public void Render(Camera camera, VertexArray vertexArray, InstancedBufferSet buffers)
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

            vertexArray.Use();

            const int triangleCount = 12;
            const int verticesPerTriangle = 3;
            GL.DrawArraysInstanced(PrimitiveType.Triangles, 0, triangleCount * verticesPerTriangle, buffers.GetObjectCount());

            // Unind SSBOs
            //buffers.positions.Use(BufferTarget.ShaderStorageBuffer);
            //GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, texturesBufferIndex + 1, buffers.positions);

            //buffers.scales.Use(BufferTarget.ShaderStorageBuffer);
            //GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, texturesBufferIndex + 2, buffers.scales);

            //buffers.textures.Use(BufferTarget.ShaderStorageBuffer);
            //GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, texturesBufferIndex + 3, buffers.textures);
        }
    }
}
