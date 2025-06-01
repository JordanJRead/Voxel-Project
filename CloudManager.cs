using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Voxel_Project
{
    internal class CloudManager
    {
        List<Cloud> clouds = new List<Cloud>();
        CubeShaderBufferSet bufferSet = new CubeShaderBufferSet();
        CubeShaderListSet listSet = new CubeShaderListSet();

        CubeMap white = new CubeMap("Images/Cubes/white.png");
        ulong whiteHandle;
        float maxDist = 500;

        float minyPos = 50;
        float maxYPos = 55;

        float cloudMinWidth = 5;
        float cloudMaxWidth = 17;

        float cloudMinHeight = 3;
        float cloudMaxHeight = 10;

        static float RandRange(float min, float max, Random r)
        {
            return (max - min) * (float)r.NextDouble() + min;
        }

        public Cloud RandCloud(Random r)
        {
            float randX = RandRange(-maxDist, maxDist, r);
            float randY = RandRange(minyPos, maxYPos, r);
            float randZ = RandRange(-maxDist, maxDist, r);

            float randXScale = RandRange(cloudMinWidth, cloudMaxWidth, r);
            float randYScale = RandRange(cloudMinHeight, cloudMaxHeight, r);
            float randZScale = RandRange(cloudMinWidth, cloudMaxWidth, r);

            return new Cloud(new Vector3(randX, randY, randZ), new Vector3(randXScale, randYScale, randZScale), 0.5f);
        }

        public CloudManager()
        {
            Random r = new Random();

            float cloudsPer100UnitsSquared = 4;

            for (int i = 0; i < cloudsPer100UnitsSquared * maxDist * maxDist * 4 / 10000.0f; ++i)
            {
                Cloud firstCloud = RandCloud(r);
                clouds.Add(firstCloud);

                if (r.Next(0, 2) == 0) // Double clouds
                {
                    Cloud secondCloud = RandCloud(r);
                    Vector3 randOffset = new Vector3(RandRange(3, 9, r), RandRange(1, 4, r), RandRange(3, 9, r));
                    secondCloud.SetPosition(firstCloud.GetPosition() + randOffset);
                    clouds.Add(secondCloud);
                }
            }

            whiteHandle = (ulong)GL.Arb.GetTextureHandle(white);
            GL.Arb.MakeTextureHandleResident(whiteHandle);

            // Set up list set
            foreach (Cloud cloud in clouds)
            {
                listSet.positions.Add(cloud.GetPosition().X);
                listSet.positions.Add(cloud.GetPosition().Y);
                listSet.positions.Add(cloud.GetPosition().Z);

                listSet.scales.Add(cloud.GetScale().X);
                listSet.scales.Add(cloud.GetScale().Y);
                listSet.scales.Add(cloud.GetScale().Z);

                listSet.textureHandles.Add(whiteHandle);
            }
        }

        public void MoveClouds(float deltaTime)
        {
            for (int i = 0; i < clouds.Count; ++i)
            {
                Cloud cloud = clouds[i];
                cloud.Move(maxDist, Vector3.UnitX, deltaTime);
                listSet.positions[i * 3 + 0] = cloud.GetPosition().X;
                listSet.positions[i * 3 + 1] = cloud.GetPosition().Y;
                listSet.positions[i * 3 + 2] = cloud.GetPosition().Z;

                listSet.scales[i * 3 + 0] = cloud.GetScale().X;
                listSet.scales[i * 3 + 1] = cloud.GetScale().Y;
                listSet.scales[i * 3 + 2] = cloud.GetScale().Z;
            }
            bufferSet.SetFromListSet(listSet);
        }

        public CubeShaderBufferSet GetBufferSet()
        {
            return bufferSet;
        }
    }
}
