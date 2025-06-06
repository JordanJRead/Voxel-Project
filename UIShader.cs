using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Voxel_Project
{
    /// <summary>
    /// Deals with rendering UI elements to the screen
    /// </summary>
    internal class UIShader : ShaderBase
    {
        VertexArray vertexArray;
        VertexBuffer vertexBuffer;
        public UIShader(string vertPath, string fragPath) : base(vertPath, fragPath)
        {
            this.Use();
            SetInt("image", 0);

            // Square
            float[] vertices =
            {
                -0.5f, -0.5f, 0, 0,
                 0.5f, -0.5f, 1, 0,
                -0.5f,  0.5f, 0, 1,

                 0.5f,  0.5f, 1, 1,
                -0.5f,  0.5f, 0, 1,
                 0.5f, -0.5f, 1, 0
            };

            vertexBuffer = new VertexBuffer(vertices, 4);
            vertexArray = new VertexArray([2, 2], vertexBuffer);
        }

        /// <summary>
        /// Draws a texture onto the screen
        /// </summary>
        /// <param name="pos">The center position of the ui element, in terms of screen widith and screen height (0-1)</param>
        /// <param name="size">The ui elemnt's width and height in terms of the screen width (0-1)</param>
        public void Draw(Texture2D texture, Vector2 pos, float size, float aspectRatio)
        {
            GL.Disable(EnableCap.CullFace);
            GL.Disable(EnableCap.DepthTest);

            this.Use();

            texture.Use(0);
            SetVec2("position", pos);
            SetFloat("scale", size);
            SetFloat("aspectRatio", aspectRatio);
            vertexArray.Use();
            GL.DrawArrays(PrimitiveType.Triangles, 0, vertexBuffer.GetVertexCount());

            GL.Enable(EnableCap.CullFace);
            GL.Enable(EnableCap.DepthTest);
        }
    }
}
