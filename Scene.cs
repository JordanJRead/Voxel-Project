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
        string filePath;

        public Scene(string filePath)
        {
            this.filePath = filePath;
            string[] fileLines = File.ReadLines(filePath).ToArray();

            foreach (string line in fileLines)
            {
                if (line == "")
                    continue;

                string[] voxelInfo = line.Split(',');
                Vector3 pos = new Vector3();
                pos.X = float.Parse(voxelInfo[0]);
                pos.Y = float.Parse(voxelInfo[1]);
                pos.Z = float.Parse(voxelInfo[2]);
                voxels.Add(new Voxel(pos, voxelInfo[3]));
            }
        }

        public void Print()
        {
            foreach (Voxel voxel in voxels)
            {
                Console.WriteLine(voxel.ToString());
            }
        }

        public void Save()
        {
            voxels[0].position = new Vector3(11, 12, 13);
            voxels[0].type = Voxel.Type.none;

            File.Delete(filePath);
            string fileSrc = "";
            foreach (Voxel voxel in voxels)
            {
                fileSrc += voxel.position.X;
                fileSrc += ',';
                fileSrc += voxel.position.Y;
                fileSrc += ',';
                fileSrc += voxel.position.Z;
                fileSrc += ',';
                fileSrc += voxel.GetTypeName();
                fileSrc += '\n';
            }

            using (StreamWriter sw = new StreamWriter(filePath))
            {
                sw.Write(fileSrc);
            }
        }
    }
}
