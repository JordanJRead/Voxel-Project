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
    /// Class for rendering voxels
    /// </summary>
    internal class InstancedVoxelShader : ShaderBase
    {
        BUF positionsBuffer = new BUF();
        BUF texturesBuffer = new BUF();
        int numOfCubes = 0;

        public InstancedVoxelShader(string vertPath, string fragPath) : base(vertPath, fragPath)
        {
            // Bind SSBOs
            positionsBuffer.Use(BufferTarget.ShaderStorageBuffer);
            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 0, positionsBuffer);

            texturesBuffer.Use(BufferTarget.ShaderStorageBuffer);
            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 1, texturesBuffer);
        }

        /// <summary>
        /// Sends voxel data to the GPU
        /// </summary>
        /// <param name="voxels"></param>
        public void UpdateGPUVoxelData(List<Voxel> voxels, TextureManager textureManager)
        {
            numOfCubes = voxels.Count;
            positionsBuffer.Use(BufferTarget.ShaderStorageBuffer);

            // Position data is stored as x1, y1, z1, x2, y2, z2...
            // because vec3 is not memory compact with SSBOs
            // and there may be differences in the memory layout between CPU and GPU
            List<float> GPUPositionData = new List<float>(numOfCubes * 3); // Reserve space for performance increase
            for (int i = 0; i < voxels.Count; i++)
            {
                GPUPositionData.Add(voxels[i].position.X);
                GPUPositionData.Add(voxels[i].position.Y);
                GPUPositionData.Add(voxels[i].position.Z);
            }

            GL.BufferData(BufferTarget.ShaderStorageBuffer, GPUPositionData.Count * sizeof(float), GPUPositionData.ToArray(), BufferUsageHint.DynamicCopy);


            texturesBuffer.Use(BufferTarget.ShaderStorageBuffer);

            List<ulong> GPUTextureHandlesData = new List<ulong>(numOfCubes);
            for (int i = 0; i < numOfCubes; i++)
            {
                GPUTextureHandlesData.Add((ulong)textureManager.GetBindlessTextureHandle(voxels[i].type));
            }

            GL.BufferData(BufferTarget.ShaderStorageBuffer, GPUTextureHandlesData.Count * sizeof(ulong), GPUTextureHandlesData.ToArray(), BufferUsageHint.DynamicCopy);
        }

        public void Render(Camera camera, VertexArray vertexArray)
        {
            this.Use();
            SetMat4("view", camera.GetViewMatrix());
            SetMat4("projection", camera.GetProjectionMatrix());

            vertexArray.Use();

            const int triangleCount = 12;
            const int verticesPerTriangle = 3;
            GL.DrawArraysInstanced(PrimitiveType.Triangles, 0, triangleCount * verticesPerTriangle, numOfCubes);
        }
    }
}
