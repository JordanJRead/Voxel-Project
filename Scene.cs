using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Voxel_Project;

namespace Voxel_Project
{
    internal class Scene
    {
        List<Voxel> voxels = new List<Voxel>();
        CubeShader cubeShader = new CubeShader("Shaders/cube.vert", "Shaders/cube.frag");
        string initialPath;

        /// <summary>
        /// Loads scene data from a file path into program memory
        /// </summary>
        public Scene(string filePath)
        {
            string projectPath = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName;
            this.initialPath = projectPath + '/' + filePath;
            
            string[] fileLines = File.ReadLines(this.initialPath).ToArray();

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

                cubeShader.UpdateVoxelData(voxels);
            }
        }

        public void Print()
        {
            foreach (Voxel voxel in voxels)
            {
                Console.WriteLine(voxel.ToString());
            }
        }

        /// <summary>
        /// Writes scene data to the same file path that the scene was loaded from, overwriting that file
        /// </summary>
        public void Save()
        {
            Save(initialPath);
        }

        /// <summary>
        /// Writes scene data to a file. If the file exists, it gets overwriten. If the file does not exist, it get created
        /// </summary>
        /// <param name="filePath"></param>
        public void Save(string filePath)
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

        public void Render(Camera camera)
        {
            cubeShader.Render(camera);
        }
    }
}
