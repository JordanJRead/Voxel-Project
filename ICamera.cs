using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace Voxel_Project
{
    internal interface ICamera
    {
        public Matrix4 GetViewMatrix();
        public Matrix4 GetProjectionMatrix();
    }
}
