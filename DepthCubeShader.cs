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
    /// Same as cube shader, but only outputs depth information (used for creating shadow maps)
    /// </summary>
    internal class DepthCubeShader : ShaderBase
    {
        uint positionsBufferIndex = 0;
        uint scalesBufferIndex = 1;
        uint texturesBufferIndex = 2;

        public DepthCubeShader(string vertPath, string fragPath) : base(vertPath, fragPath)
        {
        }

        public void Render(ICamera camera, VertexArray vertexArray, CubeShaderBufferSet cubeBuffers)
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

            vertexArray.Use();

            const int triangleCount = 12;
            const int verticesPerTriangle = 3;
            GL.DrawArraysInstanced(PrimitiveType.Triangles, 0, triangleCount * verticesPerTriangle, cubeBuffers.GetObjectCount());
            GL.Enable(EnableCap.DepthTest);
        }
    }
}
