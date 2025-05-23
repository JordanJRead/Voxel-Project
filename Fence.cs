using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace Voxel_Project
{
    internal class Fence
    {
        public enum ConnectionType
        {
            posX,
            negX,
            posZ,
            negZ
        }
        Vector3 position;
        // Connectors to other fence posts in the world. -X is left, +X is right, -Z is forward, +Z is backwards
        bool[] connections =
        {
            false,
            false,
            false,
            false
        };

        public bool GetConnection(ConnectionType type)
        {
            return connections[(int)type];
        }

        public Fence(Vector3 position)
        {
            this.position = position;
        }

        public Vector3 GetPosition()
        {
            return position;
        }

        public void MoveBy(Vector3 moveBy)
        {
            position += moveBy;
        }

        static bool FloatEquals(float a, float b)
        {
            return (MathF.Abs(a - b) < 0.0001);
        }

        /// <summary>
        /// Updates the conenctors of this fence (and another fence) based on the position of another fence.
        /// </summary>
        /// <param name="otherFence">Should be a newly added fence, as only a new fence would cause other fences to update</param>
        public void UpdateConnectors(Fence otherFence)
        {
            float dist = (this.position - otherFence.position).Length;

            // Exit if distance is too far
            if (!FloatEquals(dist, 1))
                return;

            Vector3 displacement = otherFence.position - this.position;

            // Positive X
            if (FloatEquals(displacement.X, 1))
            {
                this.connections[(int)ConnectionType.posX] = true;
                otherFence.connections[(int)ConnectionType.negX] = true;
                return; // 2 fences should only be able to connect one way
            }

            // Negative X
            if (FloatEquals(displacement.X, -1))
            {
                this.connections[(int)ConnectionType.negX] = true;
                otherFence.connections[(int)ConnectionType.posX] = true;
                return;
            }

            // Postive Z
            if (FloatEquals(displacement.Z, 1))
            {
                this.connections[(int)ConnectionType.posZ] = true;
                otherFence.connections[(int)ConnectionType.negZ] = true;
                return;
            }

            // Negative Z
            if (FloatEquals(displacement.Z, -1))
            {
                this.connections[(int)ConnectionType.negZ] = true;
                otherFence.connections[(int)ConnectionType.posZ] = true;
                return;
            }
        }

        /// <summary>
        /// Removes the connectors of a fence when another fence is deleted
        /// </summary>
        /// <param name="emptyPosition">Position of a newly-delted fence</param>
        public void RemoveConnectors(Vector3 emptyPosition)
        {
            float dist = (this.position - emptyPosition).Length;

            // Exit if distance is too far
            if (!FloatEquals(dist, 1))
                return;

            Vector3 displacement = emptyPosition - this.position;

            // Positive X
            if (FloatEquals(displacement.X, 1))
            {
                this.connections[(int)ConnectionType.posX] = false;
                return; // 2 fences should only be able to connect one way
            }

            // Negative X
            if (FloatEquals(displacement.X, -1))
            {
                this.connections[(int)ConnectionType.negX] = false;
                return;
            }

            // Postive Z
            if (FloatEquals(displacement.Z, 1))
            {
                this.connections[(int)ConnectionType.posZ] = false;
                return;
            }

            // Negative Z
            if (FloatEquals(displacement.Z, -1))
            {
                this.connections[(int)ConnectionType.negZ] = false;
                return;
            }
        }

        /// <summary>
        /// Gets the data the shader needs to draw this fence.
        /// </summary>
        /// <param name="textureManager"></param>
        /// <returns>The lists of GPU data AND the number of cubes drawn from this fence (1 to 5 inclusive)</returns>
        public (ShaderListSet, int) GetGPUData(TextureManager textureManager)
        {
            int cubeCount = 1;

            ShaderListSet listSet = new ShaderListSet();

            /// First bit deals with the fence post
            // POSITIONS
            // Position data is stored as x1, y1, z1, x2, y2, z2...
            // because vec3 is not memory compact with SSBOs
            // and there may be differences in the memory layout between CPU and GPU
            listSet.positions.Add(position.X);
            listSet.positions.Add(position.Y);
            listSet.positions.Add(position.Z);

            // SCALES
            // Scale data is stored as x1, y1, z1, x2, y2, z2...
            listSet.scales.Add(0.2f);
            listSet.scales.Add(1);
            listSet.scales.Add(0.2f);

            listSet.textureHandles.Add(textureManager.GetBindlessTextureHandle(Voxel.Type.none));

            /// This deals with the fence connectors
            if (GetConnection(Fence.ConnectionType.posX))
            {
                ++cubeCount;
                listSet.positions.Add(position.X + 0.25f);
                listSet.positions.Add(position.Y);
                listSet.positions.Add(position.Z);

                listSet.scales.Add(0.5f);
                listSet.scales.Add(0.1f);
                listSet.scales.Add(0.1f);

                listSet.textureHandles.Add(textureManager.GetBindlessTextureHandle(Voxel.Type.none));
            }

            if (GetConnection(Fence.ConnectionType.negX))
            {
                ++cubeCount;
                listSet.positions.Add(position.X - 0.25f);
                listSet.positions.Add(position.Y);
                listSet.positions.Add(position.Z);

                listSet.scales.Add(0.5f);
                listSet.scales.Add(0.1f);
                listSet.scales.Add(0.1f);

                listSet.textureHandles.Add(textureManager.GetBindlessTextureHandle(Voxel.Type.none));
            }

            if (GetConnection(Fence.ConnectionType.posZ))
            {
                ++cubeCount;
                listSet.positions.Add(position.X);
                listSet.positions.Add(position.Y);
                listSet.positions.Add(position.Z + 0.25f);

                listSet.scales.Add(0.1f);
                listSet.scales.Add(0.1f);
                listSet.scales.Add(0.5f);

                listSet.textureHandles.Add(textureManager.GetBindlessTextureHandle(Voxel.Type.none));
            }

            if (GetConnection(Fence.ConnectionType.negZ))
            {
                ++cubeCount;
                listSet.positions.Add(position.X);
                listSet.positions.Add(position.Y);
                listSet.positions.Add(position.Z - 0.25f);

                listSet.scales.Add(0.1f);
                listSet.scales.Add(0.1f);
                listSet.scales.Add(0.5f);

                listSet.textureHandles.Add(textureManager.GetBindlessTextureHandle(Voxel.Type.none));
            }

            return (listSet, cubeCount);
        }
    }
}
