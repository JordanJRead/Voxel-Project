using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Voxel_Project
{
    internal static class Vertices
    {
        public static float[] GetCubeVertices()
        {

            // Cube vertices
            // x1, y1, z1, x2, y2, z2, etc.
            // Cube is centerd on (0, 0, 0) and has dimensions of 1 (-0.5 to 0.5)
            /*
              Y
              |
              |
              |
              |
              ----------X
             /
            /
           Z
            */
            float right = 0.5f;
            float left = -0.5f;
            float up = 0.5f;
            float down = -0.5f;
            float near = 0.5f;
            float far = -0.5f;

            float[] cubeVertices =
            {
                // Front face
                left,  up,   near, 0, 0, near,
                left,  down, near, 0, 0, near,
                right, down, near, 0, 0, near,

                right, down, near, 0, 0, near,
                right, up,   near, 0, 0, near,
                left,  up,   near, 0, 0, near,

                // Back face
                right, down, far, 0, 0, far,
                left,  down, far, 0, 0, far,
                left,  up,   far, 0, 0, far,

                left,  up,   far, 0, 0, far,
                right, up,   far, 0, 0, far,
                right, down, far, 0, 0, far,

                // Right face
                right, up,   far,  right, 0, 0,
                right, up,   near, right, 0, 0,
                right, down, near, right, 0, 0,

                right, down, near, right, 0, 0,
                right, down, far,  right, 0, 0,
                right, up,   far,  right, 0, 0,
                
                // Left face
                left, down, near, left, 0, 0,
                left, up,   near, left, 0, 0,
                left, up,   far,  left, 0, 0,

                left, up,   far,  left, 0, 0,
                left, down, far,  left, 0, 0,
                left, down, near, left, 0, 0,

                // Top face
                left,  up, far,  0, up, 0,
                left,  up, near, 0, up, 0,
                right, up, near, 0, up, 0,

                right, up, near, 0, up, 0,
                right, up, far,  0, up, 0,
                left,  up, far,  0, up, 0,

                // Bottom face
                right, down, near, 0, down, 0,
                left,  down, near, 0, down, 0,
                left,  down, far,  0, down, 0,

                left,  down, far,  0, down, 0,
                right, down, far,  0, down, 0,
                right, down, near, 0, down, 0,
            };
            return cubeVertices;
        }

        private static float[] RotateQuad(float[] vertices, int degrees)
        {
            float[] newVertices = new float[vertices.Length];
            float rad = degrees * MathF.PI / 180.0f;

            for (int i = 0; i < vertices.Length / 8; ++i)
            {
                Vector3 pos = new Vector3(vertices[i * 8 + 0], vertices[i * 8 + 1], vertices[i * 8 + 2]);
                Vector3 normal = new Vector3(vertices[i * 8 + 3], vertices[i * 8 + 4], vertices[i * 8 + 5]);

                // https://math.stackexchange.com/questions/3039473/rotating-3d-points-around-a-z-axis
                newVertices[i * 8 + 0] = pos.X * MathF.Cos(rad) - pos.Z * MathF.Sin(rad);
                newVertices[i * 8 + 1] = vertices[i * 8 + 1];
                newVertices[i * 8 + 2] = pos.X * MathF.Sin(rad) + pos.Z * MathF.Cos(rad);

                newVertices[i * 8 + 3] = normal.X * MathF.Cos(rad) - normal.Z * MathF.Sin(rad);
                newVertices[i * 8 + 4] = vertices[i * 8 + 4];
                newVertices[i * 8 + 5] = normal.X * MathF.Sin(rad) + normal.Z * MathF.Cos(rad);

                newVertices[i * 8 + 6] = vertices[i * 8 + 6];
                newVertices[i * 8 + 7] = vertices[i * 8 + 7];
            }
            return newVertices;
        }

        // Plants are three quads in a billboarded pattern
        public static float[] GetPlantVertices()
        {
            float right = 0.5f;
            float left = -0.5f;
            float up = 0.5f;
            float down = -0.5f;
            float near = 0.5f;
            float far = -0.5f;

            // x, y, z, nx, ny, nz, u, v
            float[] quadVertices1 =
            {
                left,  up,   0, 0, 0, near, 0,    1.0f,
                left,  down, 0, 0, 0, near, 0,    0,
                right, down, 0, 0, 0, near, 1.0f, 0,

                right, down, 0, 0, 0, near, 1.0f, 0,
                right, up,   0, 0, 0, near, 1.0f, 1.0f,
                left,  up,   0, 0, 0, near, 0,    1.0f
            };

            float[] quadVertices2 = RotateQuad(quadVertices1, 120);
            float[] quadVertices3 = RotateQuad(quadVertices1, 240);

            quadVertices1 = quadVertices1.Concat(quadVertices2).ToArray();
            quadVertices1 = quadVertices1.Concat(quadVertices3).ToArray();

            return quadVertices1;
        }
    }
}
