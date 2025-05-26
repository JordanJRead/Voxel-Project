using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
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
        FenceManager fenceManager = new FenceManager();

        // Cube vertices
        VertexArray cubeVertexArray;
        VertexBuffer cubeVertexBuffer;

        CubeShader cubeShader = new CubeShader("Shaders/cube.vert", "Shaders/cube.frag");

        ShaderBufferSet voxelsBuffers = new ShaderBufferSet();
        ShaderBufferSet fenceBuffers = new ShaderBufferSet();

        TextureManager textureManager = new TextureManager();
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

                string[] objectInfo = line.Split(',');

                if (objectInfo[0] == "voxel")
                {
                    Vector3 pos = new Vector3();
                    pos.X = float.Parse(objectInfo[1]);
                    pos.Y = float.Parse(objectInfo[2]);
                    pos.Z = float.Parse(objectInfo[3]);
                    voxels.Add(new Voxel(pos, objectInfo[4]));
                }
                else if (objectInfo[0] == "fence")
                {
                    Vector3 pos = new Vector3();
                    pos.X = float.Parse(objectInfo[1]);
                    pos.Y = float.Parse(objectInfo[2]);
                    pos.Z = float.Parse(objectInfo[3]);
                    fenceManager.AddFence(pos);
                }
            }
            UpdateGPUVoxelData();
            UpdateGPUFenceData();

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
                right, up,   far,  right, 0, 0,
                right, up,   near, right, 0, 0,
                right, down, near, right, 0, 0,

                right, down, near, right, 0, 0,
                right, down, far,  right, 0, 0,
                right, up,   far,  right, 0, 0,
                
                // Left face
                left, down, near, left, 0, 0,
                left, up,   near, left, 0, 0,
                left, up,   far,  left, 0, 0,

                left, up,   far,  left, 0, 0,
                left, down, far,  left, 0, 0,
                left, down, near, left, 0, 0,

                // Top face
                left,  up, far,  0, up, 0,
                left,  up, near, 0, up, 0,
                right, up, near, 0, up, 0,

                right, up, near, 0, up, 0,
                right, up, far,  0, up, 0,
                left,  up, far,  0, up, 0,

                // Bottom face
                right, down, near, 0, down, 0,
                left,  down, near, 0, down, 0,
                left,  down, far,  0, down, 0,

                left,  down, far,  0, down, 0,
                right, down, far,  0, down, 0,
                right, down, near, 0, down, 0,
            };
            cubeVertexBuffer = new VertexBuffer(vertices, 6);
            cubeVertexArray = new VertexArray([3, 3], cubeVertexBuffer);
        }

        public List<Voxel> GetVoxels()
        {
            return voxels;
        }

        public List<Fence> GetFences()
        {
            return fenceManager.GetFences();
        }

        public void AddVoxel(Voxel voxel)
        {
            voxels.Add(voxel);
        }

        public FenceManager GetFenceManager()
        {
            return fenceManager;
        }

        public TextureManager GetTextureManager()
        {
            return textureManager;
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
            File.Delete(filePath);
            string fileSrc = "";
            foreach (Voxel voxel in voxels)
            {
                fileSrc += "voxel,";
                fileSrc += voxel.GetPosition().X;
                fileSrc += ',';
                fileSrc += voxel.GetPosition().Y;
                fileSrc += ',';
                fileSrc += voxel.GetPosition().Z;
                fileSrc += ',';
                fileSrc += voxel.GetTypeName();
                fileSrc += '\n';
            }

            for (int i = 0; i < fenceManager.GetCount(); ++i)
            {
                Fence fence = fenceManager[i];
                fileSrc += "fence,";
                fileSrc += fence.GetPosition().X;
                fileSrc += ',';
                fileSrc += fence.GetPosition().Y;
                fileSrc += ',';
                fileSrc += fence.GetPosition().Z;
                fileSrc += '\n';
            }

            using (StreamWriter sw = new StreamWriter(filePath))
            {
                sw.Write(fileSrc);
            }
        }

        public void Render(Camera camera, Cursor? cursor = null)
        {
            cubeShader.Render(camera, cubeVertexArray, voxelsBuffers, textureManager);
            cubeShader.Render(camera, cubeVertexArray, fenceBuffers, textureManager);
            if (cursor != null)
            {
                cubeShader.Render(camera, cubeVertexArray, cursor.GetShaderBuffers(), textureManager, true);
            }
        }

        public void RenderBufferSet(Camera camera, ShaderBufferSet bufferSet)
        {
            cubeShader.Render(camera, cubeVertexArray, bufferSet, textureManager);
        }

        /// <summary>
        /// Updates the GPU with the current scene data
        /// </summary>
        public void Update(KeyboardState keyboard, MouseState mouse) // CALL THIS FUNCTION IF RETURN VALUE OF PLAYER.UPDATE IS TRUE
        {
            UpdateGPUVoxelData();
            UpdateGPUFenceData();
        }

        /// <summary>
        /// Checks which voxel is at a specific position, if any
        /// </summary>
        /// <returns>The voxel at the given position, or null if there is no voxel there</returns>
        public Voxel? GetVoxelAtPosition(Vector3 position)
        {
            foreach (Voxel voxel in voxels)
            {
                // If positions are equal
                if ((voxel.GetPosition() - position).Length < 0.01)
                {
                    return voxel;
                }
            }
            return null;
        }

        /// <summary>
        /// Sends voxel data to the GPU
        /// </summary>
        public void UpdateGPUVoxelData()
        {
            List<float> GPUPositionData = new List<float>(voxelsBuffers.GetObjectCount() * 3); // Reserve space for performance increase
            List<float> GPUScaleData = new List<float>(voxelsBuffers.GetObjectCount() * 3);
            List<ulong> GPUTextureHandlesData = new List<ulong>(voxelsBuffers.GetObjectCount());

            for (int i = 0; i < voxels.Count; i++)
            {
                ShaderListSet listSet = voxels[i].GetGPUData(textureManager);
                GPUPositionData.AddRange(listSet.positions);
                GPUScaleData.AddRange(listSet.scales);
                GPUTextureHandlesData.AddRange(listSet.textureHandles);
            }

            voxelsBuffers.SetPositions(GPUPositionData);
            voxelsBuffers.SetScales(GPUScaleData);
            voxelsBuffers.SetTextureHandles(GPUTextureHandlesData);
        }

        /// <summary>
        /// Sends fance post and fence connector data to the GPU
        /// </summary>
        public void UpdateGPUFenceData()
        {
            int totalCubeCount = 0;

            List<float> GPUFencePositions = new List<float>(fenceBuffers.GetObjectCount() * 3); // Reserve space for performance increase
            List<float> GPUFenceScales = new List<float>(fenceBuffers.GetObjectCount() * 3); // Reserve space for performance increase
            List<ulong> GPUFenceTextureHandles = new List<ulong>(fenceBuffers.GetObjectCount());

            for (int i = 0; i < fenceManager.GetCount(); i++)
            {
                ShaderListSet listSet;
                int cubeCount;
                (listSet, cubeCount) = fenceManager[i].GetGPUData(textureManager);
                totalCubeCount += cubeCount;
                GPUFencePositions.AddRange(listSet.positions);
                GPUFenceScales.AddRange(listSet.scales);
                GPUFenceTextureHandles.AddRange(listSet.textureHandles);
            }

            fenceBuffers.SetPositions(GPUFencePositions);
            fenceBuffers.SetScales(GPUFenceScales);
            fenceBuffers.SetTextureHandles(GPUFenceTextureHandles);
        }
    }
}
