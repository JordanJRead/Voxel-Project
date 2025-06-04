using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Voxel_Project.OpenGL_Objects;
using OpenTK.Graphics.OpenGL4;

namespace Voxel_Project
{
    internal class StarShader : ShaderBase
    {
        uint matricesBufferIndex = 0;

        public StarShader(string vertPath, string fragPath) : base(vertPath, fragPath)
        {
        }

        public void Render(ICamera camera, float dayStrength, VertexArray vertexArray, int starCount, BUF ssbo)
        {
            this.Use();
            SetFloat("nightStrength", 1 - dayStrength);

            SetMat4("view", camera.GetViewMatrix());
            SetMat4("projection", camera.GetProjectionMatrix());

            ssbo.Use(BufferTarget.ShaderStorageBuffer);
            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, matricesBufferIndex, ssbo);

            vertexArray.Use();

            const int triangleCount = 12;
            const int verticesPerTriangle = 3;
            GL.DrawArraysInstanced(PrimitiveType.Triangles, 0, triangleCount * verticesPerTriangle, starCount);
        }
    }
}
