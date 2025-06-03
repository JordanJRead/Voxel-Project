using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;

namespace Voxel_Project
{
    internal class OrthoCamera : ICamera
    {
        Vector3 position;

        public OrthoCamera()
        {
        }

        public Matrix4 GetViewMatrix()
        {
            return Matrix4.LookAt(position, Vector3.Zero, Vector3.UnitY);
        }

        public Matrix4 GetProjectionMatrix()
        {
            return Matrix4.CreateOrthographic(50, 50, 0.1f, 6);
        }

        public void SetPosition(Vector3 position)
        {
            this.position = position;
            Vector4 worldPos = new Vector4(0, 8, 0, 1);
            Vector4 sunSpace = GetViewMatrix() * worldPos;
            Vector4 clipSpace = GetProjectionMatrix() * sunSpace;
            Vector3 normClipSpace = clipSpace.Xyz / clipSpace.W;
        }
    }
}
