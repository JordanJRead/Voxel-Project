using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;

namespace Voxel_Project
{
    /// <summary>
    /// Renders the sun and moon
    /// </summary>
    internal class CelestialShader : ShaderBase
    {
        public CelestialShader(string vertPath, string fragPath) : base(vertPath, fragPath)
        {

        }

        public void Render(Camera camera, VertexArray vertexArray, float dayProgress)
        {
            this.Use();
            SetMat4("view", camera.GetViewMatrix());
            SetMat4("projection", camera.GetProjectionMatrix());
            SetFloat("dayProgress", dayProgress);
            vertexArray.Use();
            const int triangleCount = 12;
            const int verticesPerTriangle = 3;

            SetBool("isSun", true);
            GL.DrawArrays(PrimitiveType.Triangles, 0, triangleCount * verticesPerTriangle);

            SetBool("isSun", false);
            GL.DrawArrays(PrimitiveType.Triangles, 0, triangleCount * verticesPerTriangle);
        }
    }
}
