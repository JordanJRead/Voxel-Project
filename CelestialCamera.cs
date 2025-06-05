using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;

namespace Voxel_Project
{
    /// <summary>
    /// The camera that the sun and moon use to render shadow maps
    /// </summary>
    internal class CelestialCamera : ICamera
    {
        Vector3 position;
        public CelestialCamera()
        {
        }

        public Matrix4 GetViewMatrix()
        {
            Matrix4 view = Matrix4.LookAt(position, Vector3.Zero, Vector3.UnitY);
            return view;
        }

        public Matrix4 GetProjectionMatrix()
        {
            return Matrix4.CreateOrthographic(100, 100, 0.1f, 150);
        }

        public void SetPosition(Vector3 position)
        {
            this.position = position;
        }

        public Vector3 GetPosition()
        {
            return this.position;
        }
    }
}
