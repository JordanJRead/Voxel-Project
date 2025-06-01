using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using System.Text;
using System.Threading.Tasks;

namespace Voxel_Project
{
    internal class CloudManager
    {
        List<Cloud> clouds = new List<Cloud>();
        CubeShaderBufferSet bufferSet = new CubeShaderBufferSet();

        CubeMap white = new CubeMap("Images/Cubes/white.png");
        ulong whiteHandle;
        float maxDist = 500;

        float cloudYPos = 50;

        float cloudMinWidth = 17;
        float cloudMaxWidth = 5;

        float cloudMinHeight = 3;
        float cloudMaxHeight = 10;

        static float RandRange(float min, float max, Random r)
        {
            return (max - min) * (float)r.NextDouble() + min;
        }

        public Cloud RandCloud(Random r)
        {
            float randX = RandRange(-maxDist, maxDist, r);
            float randZ = RandRange(-maxDist, maxDist, r);

            float randXScale = RandRange(cloudMinWidth, cloudMaxWidth, r);
            float randYScale = RandRange(cloudMinHeight, cloudMaxHeight, r);
            float randZScale = RandRange(cloudMinWidth, cloudMaxWidth, r);

            return new Cloud(new Vector3(randX, cloudYPos, randZ), new Vector3(randXScale, randYScale, randZScale), 0.5f);
        }

        public CloudManager()
        {
            Random r = new Random();

            float cloudsPer100UnitsSquared = 4;

            for (int i = 0; i < cloudsPer100UnitsSquared * maxDist * maxDist * 4 / 10000.0f; ++i)
            {
                clouds.Add(RandCloud(r));

                if (r.Next(0, 2) == 0)
                {
                    clouds.Add(RandCloud(r));
                }
            }

            whiteHandle = (ulong)GL.Arb.GetTextureHandle(white);
            GL.Arb.MakeTextureHandleResident(whiteHandle);
        }

        public void MoveClouds(float deltaTime)
        {
            CubeShaderListSet listSet = new CubeShaderListSet();
            foreach (Cloud cloud in clouds)
            {
                cloud.Move(maxDist, Vector3.UnitX, deltaTime);
                listSet.positions.Add(cloud.GetPosition().X);
                listSet.positions.Add(cloud.GetPosition().Y);
                listSet.positions.Add(cloud.GetPosition().Z);

                listSet.scales.Add(cloud.GetScale().X);
                listSet.scales.Add(cloud.GetScale().Y);
                listSet.scales.Add(cloud.GetScale().Z);

                listSet.textureHandles.Add(whiteHandle);
            }
            bufferSet.SetFromListSet(listSet);
        }

        public CubeShaderBufferSet GetBufferSet()
        {
            return bufferSet;
        }
    }
}
