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
    /// Contains all the information the shader needs for drawing several plants
    /// </summary>
    internal class PlantShaderBufferSet
    {
        public BUF positions = new BUF();
        public BUF growths = new BUF();
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

        public void SetGrowths(List<float> newGrowths)
        {
            growths.Use(BufferTarget.ShaderStorageBuffer);
            GL.BufferData(BufferTarget.ShaderStorageBuffer, newGrowths.Count * sizeof(float), newGrowths.ToArray(), BufferUsageHint.DynamicCopy);
            objectCount = newGrowths.Count;
        }

        public void SetTextureHandles(List<ulong> newTextures)
        {
            textures.Use(BufferTarget.ShaderStorageBuffer);
            GL.BufferData(BufferTarget.ShaderStorageBuffer, newTextures.Count * sizeof(ulong), newTextures.ToArray(), BufferUsageHint.DynamicCopy);
            objectCount = newTextures.Count;
        }

        public void SetFromListSet(PlantShaderListSet listSet)
        {
            SetPositions(listSet.positions);
            SetGrowths(listSet.growths);
            SetTextureHandles(listSet.textureHandles);
        }
    }
}
