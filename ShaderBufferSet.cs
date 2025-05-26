using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Voxel_Project.OpenGL_Objects;
using OpenTK.Graphics.OpenGL4;

namespace Voxel_Project
{
    /// <summary>
    /// Contains all the information the shader needs for drawing several of one type of object
    /// </summary>
    internal class ShaderBufferSet
    {
        public BUF positions = new BUF();
        public BUF scales = new BUF();
        public BUF textures = new BUF();
        private int objectCount = 0;

        public int GetObjectCount()
        {
            return objectCount;
        }

        public void SetPositions(List<float> newPositions)
        {
            positions.Use(BufferTarget.ShaderStorageBuffer);
            GL.BufferData(BufferTarget.ShaderStorageBuffer, newPositions.Count * sizeof(float), newPositions.ToArray(), BufferUsageHint.DynamicCopy);
            objectCount = newPositions.Count / 3;
        }

        public void SetScales(List<float> newScales)
        {
            scales.Use(BufferTarget.ShaderStorageBuffer);
            GL.BufferData(BufferTarget.ShaderStorageBuffer, newScales.Count * sizeof(float), newScales.ToArray(), BufferUsageHint.DynamicCopy);
            objectCount = newScales.Count / 3;
        }

        public void SetTextureHandles(List<ulong> newTextures)
        {
            textures.Use(BufferTarget.ShaderStorageBuffer);
            GL.BufferData(BufferTarget.ShaderStorageBuffer, newTextures.Count * sizeof(ulong), newTextures.ToArray(), BufferUsageHint.DynamicCopy);
            objectCount = newTextures.Count;
        }
    }
}
