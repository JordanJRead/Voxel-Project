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

        InstancedShader instancedShader = new InstancedShader("Shaders/instanced.vert", "Shaders/instanced.frag");
        Cursor cursor;

        ShaderBufferSet voxelsBuffers = new ShaderBufferSet();
        ShaderBufferSet fenceBuffers = new ShaderBufferSet();

        TextureManager textureManager = new TextureManager();
        string initialPath;

        /// <summary>
        /// Loads scene data from a file path into program memory
        /// </summary>
        public Scene(string filePath)
        {
            cursor = new Cursor(new Vector3(0, 0, 0), Voxel.Type.none, true, textureManager); // The transparent voxel that can be moved around in editor mode
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
            cubeVertexBuffer = new VertexBuffer(vertices, 6);
            cubeVertexArray = new VertexArray([3, 3], cubeVertexBuffer);
        }

        public List<Voxel> GetVoxels()
        {
            return voxels;
        }

        public void AddVoxel(Voxel voxel)
        {
            voxels.Add(voxel);
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
                fileSrc += voxel.GetPosition().X;
                fileSrc += ',';
                fileSrc += voxel.GetPosition().Y;
                fileSrc += ',';
                fileSrc += voxel.GetPosition().Z;
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
            instancedShader.Render(camera, cubeVertexArray, voxelsBuffers);
            instancedShader.Render(camera, cubeVertexArray, fenceBuffers);
            if (cursor.IsActive())
            {
                instancedShader.Render(camera, cubeVertexArray, cursor.GetShaderBuffers(), true);
            }
        }

        /// <summary>
        /// Mostly deals with moving the editor's 'selection' cube
        /// </summary>
        public void Update(KeyboardState keyboard, MouseState mouse, Camera camera)
        {
            if (cursor == null)
                return;

            if (cursor.Update(camera, keyboard, mouse, this, textureManager))
            {
                UpdateGPUVoxelData();
            }
        }

        /// <summary>
        /// Checks which voxel the editor's cursor is overlapping, if any
        /// </summary>
        /// <returns>The overlapped voxel</returns>
        public Voxel? GetSelectedVoxel()
        {
            if (cursor == null)
                return null;

            foreach (Voxel voxel in voxels)
            {
                // If positions are equal
                if ((voxel.GetPosition() - cursor.GetPosition()).Length < 0.01)
                {
                    return voxel;
                }
            }
            return null;
        }

        /// <summary>
        /// Checks which fence the editor's cursor is overlapping, if any
        /// </summary>
        /// <returns>The overlapped fence</returns>
        public Fence? GetSelectedFence()
        {
            if (cursor == null)
                return null;

            for (int i = 0; i < fenceManager.GetCount(); ++i)
            {
                Fence fence = fenceManager[i];
                if ((fence.GetPosition() - cursor.GetPosition()).Length < 0.01)
                {
                    return fence;
                }
            }
            return null;
        }

        /// <summary>
        /// Sends voxel data to the GPU
        /// </summary>
        public void UpdateGPUVoxelData()
        {
            voxelsBuffers.SetObjectCount(voxels.Count);

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
            fenceBuffers.SetObjectCount(fenceManager.GetCount());

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

            fenceBuffers.SetObjectCount(totalCubeCount);
            fenceBuffers.SetPositions(GPUFencePositions);
            fenceBuffers.SetScales(GPUFenceScales);
            fenceBuffers.SetTextureHandles(GPUFenceTextureHandles);
        }
    }
}
