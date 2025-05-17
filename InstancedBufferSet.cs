using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Voxel_Project.OpenGL_Objects;

namespace Voxel_Project
{
    /// <summary>
    /// Contains all the information the shader needs for drawing a bunch of one type of object
    /// </summary>
    internal class InstancedBufferSet
    {
        public BUF positions = new BUF();
        public BUF scales = new BUF();
        public BUF textures = new BUF();
        int objectCount = 0;

        public void SetObjectCount(int objectCount)
        {
            this.objectCount = objectCount;
        }

        public int GetObjectCount()
        {
            return objectCount;
        }
    }
}
