using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;

namespace Voxel_Project
{
    internal class CelestialCamera : ICamera
    {
        Vector3 position;
        public CelestialCamera()
        {
        }

        public Matrix4 GetViewMatrix()
        {
            return Matrix4.LookAt(position, Vector3.Zero, Vector3.UnitY);
        }

        public Matrix4 GetProjectionMatrix()
        {
            return Matrix4.CreateOrthographic(100, 100, 0.1f, 20);
        }

        public void SetPosition(Vector3 position)
        {
            this.position = position;
        }
    }
}
