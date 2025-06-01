using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace Voxel_Project
{
    internal class Cloud
    {
        Vector3 position;
        Vector3 scale;
        float speed;

        public Cloud(Vector3 position, Vector3 scale, float speed)
        {
            this.position = position;
            this.scale = scale;
            this.speed = speed;
        }

        public Vector3 GetPosition()
        {
            return position;
        }

        public Vector3 GetScale()
        {
            return scale;
        }

        public void SetPosition(Vector3 position)
        {
            this.position = position;
        }

        /// <summary>
        /// Moves the cloud by its speed
        /// </summary>
        /// <param name="maxDist">The max absolute x or z position a cloud can be at</param>
        public void Move(float maxDist, Vector3 direction, float deltaTime)
        {
            position += direction * deltaTime * speed;
            
            if (position.X > maxDist)
            {
                position.X -= maxDist * 2;
            }
            if (position.X < -maxDist)
            {
                position.X += maxDist * 2;
            }

            if (position.Z > maxDist)
            {
                position.Z -= maxDist * 2;
            }
            if (position.Z < -maxDist)
            {
                position.Z += maxDist * 2;
            }
        }
    }
}
