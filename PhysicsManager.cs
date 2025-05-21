using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Voxel_Project
{
    internal class PhysicsManager
    {
        class AABB
        {
            public Vector3 min;
            public Vector3 max;

            public AABB(Vector3 min, Vector3 max)
            {
                this.min = min;
                this.max = max;
            }

            public bool Intersects(AABB other)
            {
                return !(this.min.X > other.max.X || this.max.X < other.min.X || this.min.Y > other.max.Y || this.max.Y < other.max.Y || this.min.Z > other.max.Z || this.max.Z < other.max.Z);
            }
        }

        public Vector3 MoveInScene(PlayerCamera playerCamera, Scene scene, Vector3 displacement)
        {
            Vector3 desiredPlayerPosition = playerCamera.GetPosition() + displacement;
            AABB playerAABB = new AABB(desiredPlayerPosition - new Vector3(0.5f), desiredPlayerPosition + new Vector3(0.5f));
            foreach (Voxel voxel in scene.GetVoxels())
            {
                AABB voxelAABB = new AABB(voxel.GetPosition() - new Vector3(0.5f), voxel.GetPosition() + new Vector3(0.5f));
                if (playerAABB.Intersects(voxelAABB))
                {
                    return playerCamera.GetPosition();
                }
            }
            return playerCamera.GetPosition() + displacement;
        }
    }
}
