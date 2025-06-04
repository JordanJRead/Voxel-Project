using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Voxel_Project.OpenGL_Objects;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Voxel_Project
{
    internal class StarManager
    {
        List<Star> stars = new List<Star>();
        int starCount = 1000;
        BUF matrixSSBO;

        public StarManager()
        {
            matrixSSBO = new BUF();
            matrixSSBO.Use(BufferTarget.ShaderStorageBuffer);

            float dist = 10000;
            float minScale = 20f;
            float maxScale = 50f;

            for (int i = 0; i < starCount; ++i)
            {
                stars.Add(new Star(dist, minScale, maxScale));
            }

            List<Matrix4> matrices = new List<Matrix4>();

            foreach (Star star in stars)
            {
                matrices.Add(star.GetModelMatrix());
            }

            GL.BufferData(BufferTarget.ShaderStorageBuffer, matrices.Count * 4 * 4 * sizeof(float), matrices.ToArray(), BufferUsageHint.StaticRead);
        }

        public int GetStarCount()
        {
            return starCount;
        }

        public BUF GetSSBO()
        {
            return matrixSSBO;
        }
    }
}
