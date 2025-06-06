using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using Voxel_Project.OpenGL_Objects;

namespace Voxel_Project
{
    /// <summary>
    /// Shader for rendering a texture to cover the entire screen, mostly for graphics debugging
    /// </summary>
    internal class ScreenTextureShader : ShaderBase
    {
        VertexArray screenQuadVertexArray;
        VertexBuffer screenQuadVertexBuffer;
        public ScreenTextureShader(string vertPath, string fragPath) : base(vertPath, fragPath)
        {
            this.Use();
            SetInt("image", 0);

            float[] vertices =
            {
                -1.0f, -1.0f, 0.0f, 0.0f,
                -1.0f,  1.0f, 0.0f, 1.0f,
                 1.0f, -1.0f, 1.0f, 0.0f,

                -1.0f,  1.0f, 0.0f, 1.0f,
                 1.0f, -1.0f, 1.0f, 0.0f,
                 1.0f,  1.0f, 1.0f, 1.0f,
            };

            screenQuadVertexBuffer = new VertexBuffer(vertices, 4);
            screenQuadVertexArray = new VertexArray([2, 2], screenQuadVertexBuffer);
        }

        /// <summary>
        /// Renders a texture to cover the screen
        /// </summary>
        public void Render(Texture2D texture)
        {
            GL.Disable(EnableCap.DepthTest);
            GL.Disable(EnableCap.CullFace);
            this.Use();

            texture.Use(0);

            screenQuadVertexArray.Use();

            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
        }
    }
}
