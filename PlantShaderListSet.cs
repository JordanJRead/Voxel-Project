using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Voxel_Project
{
    /// <summary>
    /// Contains lists for each of the data types the GPU needs for the plant shader. Similar to PlantShaderBufferSet
    /// </summary>
    internal class PlantShaderListSet
    {
        public List<float> positions = new List<float>();
        public List<float> growths = new List<float>();
        public List<ulong> textureHandles = new List<ulong>();
    }
}
