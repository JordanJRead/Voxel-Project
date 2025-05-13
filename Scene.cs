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
        Voxel? editorVoxel = null;

        VertexArray vertexArray;
        VertexBuffer vertexBuffer;

        InstancedVoxelShader instancedVoxelShader = new InstancedVoxelShader("Shaders/instancedvoxel.vert", "Shaders/instancedvoxel.frag");
        TransparentVoxelShader transparentVoxelShader = new TransparentVoxelShader("Shaders/transparentvoxel.vert", "Shaders/transparentvoxel.frag");
        TextureManager textureManager = new TextureManager();
        string initialPath;

        /// <summary>
        /// Loads scene data from a file path into program memory
        /// </summary>
        public Scene(string filePath)
        {
            editorVoxel = new Voxel(new Vector3(0, 3, 0), Voxel.Type.grass);
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
            }

            instancedVoxelShader.UpdateVoxelData(voxels, textureManager);
            // Cube vertices
            // x1, y1, z1, x2, y2, z2, etc.
            // Cube is centerd on (0, 0, 0) and has dimensions of 1 (-0.5 to 0.5)
            /*
              Y
              |
              |
              |
              |
              ----------X
             /
            /
           Z
            */
            float right = 0.5f;
            float left = -0.5f;
            float up = 0.5f;
            float down = -0.5f;
            float near = 0.5f;
            float far = -0.5f;

            float[] vertices =
            {
                // Front face
                left,  up,   near, 0, 0, near,
                left,  down, near, 0, 0, near,
                right, down, near, 0, 0, near,

                right, down, near, 0, 0, near,
                right, up,   near, 0, 0, near,
                left,  up,   near, 0, 0, near,

                // Back face
                right, down, far, 0, 0, far,
                left,  down, far, 0, 0, far,
                left,  up,   far, 0, 0, far,

                left,  up,   far, 0, 0, far,
                right, up,   far, 0, 0, far,
                right, down, far, 0, 0, far,

                // Right face
                right, up,   far, right, 0, 0,
                right, up,   near, right, 0, 0,
                right, down, near, right, 0, 0,

                right, down, near, right, 0, 0,
                right, down, far, right, 0, 0,
                right, up,   far, right, 0, 0,
                
                // Left face
                left, down, near, left, 0, 0,
                left, up,   near, left, 0, 0,
                left, up,   far, left, 0, 0,

                left, up,   far, left, 0, 0,
                left, down, far, left, 0, 0,
                left, down, near, left, 0, 0,

                // Top face
                left,  up, far, 0, up, 0,
                left,  up, near, 0, up, 0,
                right, up, near, 0, up, 0,

                right, up, near, 0, up, 0,
                right, up, far, 0, up, 0,
                left,  up, far, 0, up, 0,

                // Bottom face
                right, down, near, 0, down, 0,
                left,  down, near, 0, down, 0,
                left,  down, far, 0, down, 0,

                left,  down, far, 0, down, 0,
                right, down, far, 0, down, 0,
                right, down, near, 0, down, 0,
            };
            vertexBuffer = new VertexBuffer(vertices);
            vertexArray = new VertexArray([3, 3], vertexBuffer);
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
            instancedVoxelShader.Render(camera, vertexArray);
            if (editorVoxel != null)
            {
                transparentVoxelShader.Render(camera, vertexArray, editorVoxel, textureManager);
            }
        }
    }
}
