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
    }
}
