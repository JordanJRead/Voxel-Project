using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Voxel_Project
{
    internal class Star
    {
        static Random r = new Random();

        Matrix4 modelMatrix;

        /// <summary>
        /// https://stackoverflow.com/a/218600
        /// </summary>
        static float RandGaussian()
        {
            float mean = 0;
            float stdDev = 1;

            float u1 = 1.0f - (float)r.NextDouble(); //uniform(0,1] random doubles
            float u2 = 1.0f - (float)r.NextDouble();
            float randStdNormal = MathF.Sqrt(-2.0f * MathF.Log(u1)) * MathF.Sin(2.0f * MathF.PI * u2); //random normal(0,1)
            float randNormal = mean + stdDev * randStdNormal; //random normal(mean,stdDev^2)
            return randNormal;
        }

        /// <summary>
        /// Creates a random direction vector (length of 1)
        /// https://stackoverflow.com/a/8453514
        /// </summary>
        static Vector3 RandVector()
        {
            Vector3 vector = new Vector3(RandGaussian(), RandGaussian(), RandGaussian());
            return vector.Normalized();
        }

        public Star(float dist, float minScale, float maxScale)
        {
            Vector3 position = RandVector() * dist;
            float scale = minScale + (float)r.NextDouble() * (maxScale - minScale);
            Vector3 rotation = new Vector3((float)r.NextDouble() * 2 * MathF.PI, (float)r.NextDouble() * 2 * MathF.PI, (float)r.NextDouble() * 2 * MathF.PI);

            modelMatrix = Matrix4.Identity;
            modelMatrix *= Matrix4.CreateScale(scale);
            modelMatrix *= Matrix4.CreateFromQuaternion(new Quaternion(rotation));
            modelMatrix *= Matrix4.CreateTranslation(position);
        }

        public Matrix4 GetModelMatrix()
        {
            return modelMatrix;
        }
    }
}
