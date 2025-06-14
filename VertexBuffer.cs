﻿using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Voxel_Project
{
    /// <summary>
    /// Holds onto GPU vertex data
    /// </summary>
    internal class VertexBuffer
    {
        OpenGL_Objects.BUF buf = new OpenGL_Objects.BUF();
        int vertexCount;

        /// <summary>
        /// Sends vertex data over to the GPU and stores the name of the created buffer
        /// </summary>
        public VertexBuffer(float[] vertices, int floatsPerVertex)
        {
            vertexCount = vertices.Length / floatsPerVertex;
            buf.Use(BufferTarget.ArrayBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
        }

        public void Use()
        {
            buf.Use(BufferTarget.ArrayBuffer);
        }

        public int GetVertexCount()
        {
            return vertexCount;
        }
    }
}
