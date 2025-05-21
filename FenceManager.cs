using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace Voxel_Project
{
    /// <summary>
    /// A list of fences that also automatically updates fence connections
    /// </summary>
    internal class FenceManager
    {
        List<Fence> fences = new List<Fence>();

        public int GetCount()
        {
            return fences.Count;
        }

        public Fence this[int i]
        {
            get => fences[i];
        }

        /// <summary>
        /// Makes fences in the scene think that there is fence at a given position. Used with the editor cursor to preview fence connections
        /// </summary>
        /// <param name="position"></param>
        /// <returns>The fence that would exist at the given position, with connections</returns>
        public Fence FakeFence(Vector3 position)
        {
            Fence fakeFence = new Fence(position);
            foreach (Fence fence in fences)
            {
                fence.UpdateConnectors(fakeFence);
            }
            return fakeFence;
        }

        /// <summary>
        /// Makes sure that if there is no fence at the given position, the other fences aren't connected to the empty position
        /// Should be called to cancel out FakeFence()
        /// </summary>
        /// <param name="position"></param>
        public void UnFakeFence(Vector3 position)
        {
            if (GetFenceAtPosition(position) == null)
            {
                foreach (Fence fence in fences)
                {
                    fence.RemoveConnectors(position);
                }
            }
        }

        public void AddFence(Fence newFence)
        {
            foreach (Fence fence in fences)
            {
                fence.UpdateConnectors(newFence);
            }
            fences.Add(newFence);
        }

        public void AddFence(Vector3 position)
        {
            AddFence(new Fence(position));
        }

        public void RemoveFence(Fence newFence)
        {
            Vector3 position = newFence.GetPosition();
            fences.Remove(newFence);
            foreach (Fence fence in fences)
            {
                fence.RemoveConnectors(position);
            }
        }


        /// <summary>
        /// Checks which fence is at a specific position, if any
        /// </summary>
        /// <returns>The fence at the given position, or null if there is no fence there</returns>
        public Fence? GetFenceAtPosition(Vector3 position)
        {
            for (int i = 0; i < GetCount(); ++i)
            {
                Fence fence = fences[i];
                if ((fence.GetPosition() - position).Length < 0.01)
                {
                    return fence;
                }
            }
            return null;
        }
    }
}
