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
        FenceList fences = new FenceList();
        CursorVoxel? cursorVoxel = null; // The transparent voxel that can be moved around in editor mode

        // Cube vertices
        VertexArray cubeVertexArray;
        VertexBuffer cubeVertexBuffer;

        InstancedShader instancedShader = new InstancedShader("Shaders/instanced.vert", "Shaders/instanced.frag");
        TransparentVoxelShader transparentVoxelShader = new TransparentVoxelShader("Shaders/transparentvoxel.vert", "Shaders/transparentvoxel.frag");

        InstancedBufferSet voxelsBuffers = new InstancedBufferSet();
        InstancedBufferSet fencePostsBuffers = new InstancedBufferSet();
        InstancedBufferSet fenceConnectorsBuffers = new InstancedBufferSet();

        TextureManager textureManager = new TextureManager();
        string initialPath;

        /// <summary>
        /// Loads scene data from a file path into program memory
        /// </summary>
        public Scene(string filePath)
        {
            cursorVoxel = new CursorVoxel(new Vector3(0, 3, 0), Voxel.Type.grass);

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
            instancedShader.Render(camera, cubeVertexArray, fencePostsBuffers);
            instancedShader.Render(camera, cubeVertexArray, fenceConnectorsBuffers);
            if (cursorVoxel != null)
            {
                transparentVoxelShader.Render(camera, cubeVertexArray, cursorVoxel, textureManager);
            }
        }

        /// <summary>
        /// Mostly deals with moving the editor's 'selection' cube
        /// </summary>
        public void Update(KeyboardState keyboard, MouseState mouse, Camera camera)
        {
            if (cursorVoxel == null)
                return;

            if (cursorVoxel.Update(camera, keyboard, mouse, this))
            {
                UpdateGPUVoxelData();
            }
        }

        /// <summary>
        /// Checks which voxel the editor's cursor is overlapping, if any
        /// </summary>
        /// <returns></returns>
        public Voxel? GetSelectedVoxel()
        {
            if (cursorVoxel == null)
                return null;

            foreach (Voxel voxel in voxels)
            {
                // If positions are equal
                if ((voxel.GetPosition() - cursorVoxel.GetPosition()).Length < 0.01)
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
            voxelsBuffers.SetObjectCount(voxels.Count);

            List<float> GPUPositionData = new List<float>(voxelsBuffers.GetObjectCount() * 3); // Reserve space for performance increase
            List<float> GPUScaleData = new List<float>(voxelsBuffers.GetObjectCount() * 3);
            List<ulong> GPUTextureHandlesData = new List<ulong>(voxelsBuffers.GetObjectCount());

            // POSITIONS
            // Position data is stored as x1, y1, z1, x2, y2, z2...
            // because vec3 is not memory compact with SSBOs
            // and there may be differences in the memory layout between CPU and GPU
            for (int i = 0; i < voxels.Count; i++)
            {
                GPUPositionData.Add(voxels[i].GetPosition().X);
                GPUPositionData.Add(voxels[i].GetPosition().Y);
                GPUPositionData.Add(voxels[i].GetPosition().Z);
            }

            // SCALES
            // Scale data is stored as x1, y1, z1, x2, y2, z2...
            for (int i = 0; i < voxels.Count; i++)
            {
                GPUScaleData.Add(1);
                GPUScaleData.Add(1);
                GPUScaleData.Add(1);
            }

            // TEXTURES
            for (int i = 0; i < voxelsBuffers.GetObjectCount(); i++)
            {
                GPUTextureHandlesData.Add((ulong)textureManager.GetBindlessTextureHandle(voxels[i].type));
            }

            voxelsBuffers.positions.Use(BufferTarget.ShaderStorageBuffer);
            GL.BufferData(BufferTarget.ShaderStorageBuffer, GPUPositionData.Count * sizeof(float), GPUPositionData.ToArray(), BufferUsageHint.DynamicCopy);

            voxelsBuffers.scales.Use(BufferTarget.ShaderStorageBuffer);
            GL.BufferData(BufferTarget.ShaderStorageBuffer, GPUScaleData.Count * sizeof(float), GPUScaleData.ToArray(), BufferUsageHint.DynamicCopy);

            voxelsBuffers.textures.Use(BufferTarget.ShaderStorageBuffer);
            GL.BufferData(BufferTarget.ShaderStorageBuffer, GPUTextureHandlesData.Count * sizeof(ulong), GPUTextureHandlesData.ToArray(), BufferUsageHint.DynamicCopy);
        }

        /// <summary>
        /// Sends fance post and fence connector data to the GPU
        /// </summary>
        public void UpdateGPUFenceData()
        {
            fencePostsBuffers.SetObjectCount(fences.GetCount());

            List<float> GPUFencePostPositions = new List<float>(fencePostsBuffers.GetObjectCount() * 3); // Reserve space for performance increase
            List<float> GPUFencePostScales = new List<float>(fencePostsBuffers.GetObjectCount() * 3); // Reserve space for performance increase
            List<ulong> GPUFencePostTextureHandles = new List<ulong>(fencePostsBuffers.GetObjectCount());

            // POSITIONS
            // Position data is stored as x1, y1, z1, x2, y2, z2...
            // because vec3 is not memory compact with SSBOs
            // and there may be differences in the memory layout between CPU and GPU
            for (int i = 0; i < fences.GetCount(); i++)
            {
                GPUFencePostPositions.Add(fences[i].GetPosition().X);
                GPUFencePostPositions.Add(fences[i].GetPosition().Y);
                GPUFencePostPositions.Add(fences[i].GetPosition().Z);
            }

            // SCALES
            // Position data is stored as x1, y1, z1, x2, y2, z2...
            for (int i = 0; i < fences.GetCount(); i++)
            {
                GPUFencePostScales.Add(0.2f);
                GPUFencePostScales.Add(1);
                GPUFencePostScales.Add(0.2f);
            }

            // TEXTURES
            for (int i = 0; i < fencePostsBuffers.GetObjectCount(); i++)
            {
                GPUFencePostTextureHandles.Add((ulong)textureManager.GetBindlessTextureHandle(Voxel.Type.none));
            }

            fencePostsBuffers.positions.Use(BufferTarget.ShaderStorageBuffer);
            GL.BufferData(BufferTarget.ShaderStorageBuffer, GPUFencePostPositions.Count * sizeof(float), GPUFencePostPositions.ToArray(), BufferUsageHint.DynamicCopy);

            fencePostsBuffers.scales.Use(BufferTarget.ShaderStorageBuffer);
            GL.BufferData(BufferTarget.ShaderStorageBuffer, GPUFencePostScales.Count * sizeof(float), GPUFencePostScales.ToArray(), BufferUsageHint.DynamicCopy);

            fencePostsBuffers.textures.Use(BufferTarget.ShaderStorageBuffer);
            GL.BufferData(BufferTarget.ShaderStorageBuffer, GPUFencePostTextureHandles.Count * sizeof(ulong), GPUFencePostTextureHandles.ToArray(), BufferUsageHint.DynamicCopy);

            // CONNECTORS
            int connectorCount = 0;

            List<float> GPUFenceConnectorPositions = new List<float>();
            List<float> GPUFenceConnectorScales = new List<float>();
            List<ulong> GPUFenceConnectorTextureHandles = new List<ulong>();

            for (int i = 0; i < fences.GetCount(); ++i)
            {
                Fence fence = fences[i];

                if (fence.GetConnection(Fence.ConnectionType.posX))
                {
                    ++connectorCount;
                    GPUFenceConnectorPositions.Add(fence.GetPosition().X + 0.25f);
                    GPUFenceConnectorPositions.Add(fence.GetPosition().Y);
                    GPUFenceConnectorPositions.Add(fence.GetPosition().Z);

                    GPUFenceConnectorScales.Add(0.5f);
                    GPUFenceConnectorScales.Add(0.1f);
                    GPUFenceConnectorScales.Add(0.1f);

                    GPUFenceConnectorTextureHandles.Add((ulong)textureManager.GetBindlessTextureHandle(Voxel.Type.none));
                }

                if (fence.GetConnection(Fence.ConnectionType.negX))
                {
                    ++connectorCount;
                    GPUFenceConnectorPositions.Add(fence.GetPosition().X - 0.25f);
                    GPUFenceConnectorPositions.Add(fence.GetPosition().Y);
                    GPUFenceConnectorPositions.Add(fence.GetPosition().Z);

                    GPUFenceConnectorScales.Add(0.5f);
                    GPUFenceConnectorScales.Add(0.1f);
                    GPUFenceConnectorScales.Add(0.1f);

                    GPUFenceConnectorTextureHandles.Add((ulong)textureManager.GetBindlessTextureHandle(Voxel.Type.none));
                }

                if (fence.GetConnection(Fence.ConnectionType.posZ))
                {
                    ++connectorCount;
                    GPUFenceConnectorPositions.Add(fence.GetPosition().X);
                    GPUFenceConnectorPositions.Add(fence.GetPosition().Y);
                    GPUFenceConnectorPositions.Add(fence.GetPosition().Z + 0.25f);

                    GPUFenceConnectorScales.Add(0.1f);
                    GPUFenceConnectorScales.Add(0.1f);
                    GPUFenceConnectorScales.Add(0.5f);

                    GPUFenceConnectorTextureHandles.Add((ulong)textureManager.GetBindlessTextureHandle(Voxel.Type.none));
                }

                if (fence.GetConnection(Fence.ConnectionType.negZ))
                {
                    ++connectorCount;
                    GPUFenceConnectorPositions.Add(fence.GetPosition().X);
                    GPUFenceConnectorPositions.Add(fence.GetPosition().Y);
                    GPUFenceConnectorPositions.Add(fence.GetPosition().Z - 0.25f);

                    GPUFenceConnectorScales.Add(0.1f);
                    GPUFenceConnectorScales.Add(0.1f);
                    GPUFenceConnectorScales.Add(0.5f);

                    GPUFenceConnectorTextureHandles.Add((ulong)textureManager.GetBindlessTextureHandle(Voxel.Type.none));
                }
            }
            
            fenceConnectorsBuffers.SetObjectCount(connectorCount);

            fenceConnectorsBuffers.positions.Use(BufferTarget.ShaderStorageBuffer);
            GL.BufferData(BufferTarget.ShaderStorageBuffer, GPUFenceConnectorPositions.Count * sizeof(float), GPUFenceConnectorPositions.ToArray(), BufferUsageHint.DynamicCopy);
            
            fenceConnectorsBuffers.scales.Use(BufferTarget.ShaderStorageBuffer);
            GL.BufferData(BufferTarget.ShaderStorageBuffer, GPUFenceConnectorScales.Count * sizeof(float), GPUFenceConnectorScales.ToArray(), BufferUsageHint.DynamicCopy);

            fenceConnectorsBuffers.textures.Use(BufferTarget.ShaderStorageBuffer);
            GL.BufferData(BufferTarget.ShaderStorageBuffer, GPUFenceConnectorTextureHandles.Count * sizeof(ulong), GPUFenceConnectorTextureHandles.ToArray(), BufferUsageHint.DynamicCopy);
        }
    }
}
