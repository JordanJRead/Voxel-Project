﻿using Voxel_Project.OpenGL_Objects;
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
    /// Same as plant shader, but only outputs depth information (used for creating shadow maps)
    /// </summary>
    internal class DepthPlantShader : ShaderBase
    {
        uint positionsBufferIndex = 3;
        uint growthsBufferIndex = 4;
        uint textureHanglesBufferIndex = 5;

        public DepthPlantShader(string vertPath, string fragPath) : base(vertPath, fragPath)
        {
        }

        public void Render(ICamera camera, VertexArray vertexArray, PlantShaderBufferSet buffers, float time)
        {
            GL.Disable(EnableCap.CullFace);
            
            // Bind SSBOs
            buffers.positions.Use(BufferTarget.ShaderStorageBuffer);
            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, positionsBufferIndex, buffers.positions);

            buffers.growths.Use(BufferTarget.ShaderStorageBuffer);
            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, growthsBufferIndex, buffers.growths);

            buffers.textures.Use(BufferTarget.ShaderStorageBuffer);
            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, textureHanglesBufferIndex, buffers.textures);

            this.Use();
            SetMat4("view", camera.GetViewMatrix());
            SetMat4("projection", camera.GetProjectionMatrix());
            SetFloat("time", time);

            vertexArray.Use();

            const int triangleCount = 6;
            const int verticesPerTriangle = 3;
            GL.DrawArraysInstanced(PrimitiveType.Triangles, 0, triangleCount * verticesPerTriangle, buffers.GetObjectCount());

            GL.Enable(EnableCap.CullFace);
        }
    }
}
