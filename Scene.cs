using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Voxel_Project;

namespace OpenTK_Test
{
    internal class Scene
    {
        List<Voxel> voxels = new List<Voxel>();

        public Scene(string filePath)
        {
            string[] fileLines = File.ReadLines(filePath).ToArray();

            foreach (string line in fileLines)
            {
                // todo check if line is empty
                string[] voxelInfo = line.Split(',');
                Vector3 pos = new Vector3();
                pos.X = float.Parse(voxelInfo[0]);
                pos.Y = float.Parse(voxelInfo[1]);
                pos.Z = float.Parse(voxelInfo[2]);
                voxels.Add(new Voxel(pos, Voxel.VoxelTypeFromString(voxelInfo[3])));
            }
        }

        public void Save(string filePath)
        {
            File.Delete(filePath);
            string fileData = "";
            foreach (Voxel voxel in voxels)
            {
                fileData += 
            }
        }
    }
}
