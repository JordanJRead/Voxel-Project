using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using Voxel_Project.OpenGL_Objects;

namespace Voxel_Project
{
    internal class VertexArray
    {
        OpenGL_Objects.VAO vao = new OpenGL_Objects.VAO();

        /// <param name="floatAttributeCounts">floatAttributeCounts[i] contains the number of floats in the ith attribute.
        /// For example, a vertex with vec3 pos, vec3 normal, and vec2 uvs would have the array [3, 3, 2]
        /// </param>
        public VertexArray(int[] floatAttributeCounts, VertexBuffer vertexBuffer)
        {
            vertexBuffer.Use();
            vao.Use();
            for (int i = 0; i < floatAttributeCounts.Length; ++i)
            {
                int prevSum = 0;
                for (int j = 0; j < i; ++j)
                {
                    prevSum += floatAttributeCounts[j];
                }

                GL.EnableVertexAttribArray(i);
                GL.VertexAttribPointer(i, floatAttributeCounts[i], VertexAttribPointerType.Float, false, floatAttributeCounts.Sum() * sizeof(float), prevSum * sizeof(float));
            }
        }

        public void Use()
        {
            vao.Use();
        }
    }
}
