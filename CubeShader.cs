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
    internal class CubeShader : ShaderBase
    {
        VertexArray vertexArray;
        VertexBuffer vertexBuffer;
        OpenGL_Objects.BUF positionsBuffer = new OpenGL_Objects.BUF();
        OpenGL_Objects.BUF texturesBuffer = new OpenGL_Objects.BUF();
        int numOfCubes = 0;

        public CubeShader(string vertPath, string fragPath) : base(vertPath, fragPath)
        {
            // Bind SSBOs
            positionsBuffer.Use(BufferTarget.ShaderStorageBuffer);
            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 0, positionsBuffer);

            texturesBuffer.Use(BufferTarget.ShaderStorageBuffer);
            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 1, texturesBuffer);

            // Cube vertices
            // x1, y1, z1, x2, y2, z2, etc.
            // Cube is centerd on (0, 0, 0) and has dimensions of 1 (-0.5 to 0.5)
            /*
              Y
              |
              |
              |
              |
              ----------X
             /
            /
           Z
            */
            float right = 0.5f;
            float left = -0.5f;
            float up = 0.5f;
            float down = -0.5f;
            float near = 0.5f;
            float far = -0.5f;

            float[] vertices =
            {
                // Front face
                left,  up,   near,
                left,  down, near,
                right, down, near,

                right, down, near,
                right, up,   near,
                left,  up,   near,

                // Back face
                right, down, far,
                left,  down, far,
                left,  up,   far,

                left,  up,   far,
                right, up,   far,
                right, down, far,

                // Right face
                right, up,   far,
                right, up,   near,
                right, down, near,

                right, down, near,
                right, down, far,
                right, up,   far,
                
                // Left face
                left, down, near,
                left, up,   near,
                left, up,   far,

                left, up,   far,
                left, down, far,
                left, down, near,

                // Top face
                left,  up, far,
                left,  up, near,
                right, up, near,

                right, up, near,
                right, up, far,
                left,  up, far,

                // Bottom face
                right, down, near,
                left,  down, near,
                left,  down, far,

                left,  down, far,
                right, down, far,
                right, down, near,
            };
            vertexBuffer = new VertexBuffer(vertices);
            vertexArray = new VertexArray([3], vertexBuffer);
        }

        /// <summary>
        /// Sends voxel data to the GPU
        /// </summary>
        /// <param name="voxels"></param>
        public void UpdateVoxelData(List<Voxel> voxels, TextureManager textureManager)
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

            List<long> GPUTextureHandlesData = new List<long>(numOfCubes);
            for (int i = 0; i < numOfCubes; i++)
            {
                GPUTextureHandlesData.Add(textureManager.GetTextureHandle(voxels[i].type));
            }

            GL.BufferData(BufferTarget.ShaderStorageBuffer, GPUTextureHandlesData.Count * sizeof(float), GPUTextureHandlesData.ToArray(), BufferUsageHint.DynamicCopy);
        }

        public void Render(Camera camera)
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
