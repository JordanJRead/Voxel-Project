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

        // Plant vertices
        VertexArray plantVertexArray;
        VertexBuffer plantVertexBuffer;

        PlantManager plantManager = new PlantManager();

        CubeShader cubeShader = new CubeShader("Shaders/cube.vert", "Shaders/cube.frag");
        PlantShader plantShader = new PlantShader("Shaders/plant.vert", "Shaders/plant.frag");

        CubeShaderBufferSet voxelsBuffers = new CubeShaderBufferSet();
        CubeShaderBufferSet fenceBuffers = new CubeShaderBufferSet();

        CubeTextureManager cubeTextureManager = new CubeTextureManager();
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
                else if (objectInfo[0] == "plant")
                {
                    Vector3 pos = new Vector3();
                    pos.X = float.Parse(objectInfo[1]);
                    pos.Y = float.Parse(objectInfo[2]);
                    pos.Z = float.Parse(objectInfo[3]);
                    float growth = float.Parse(objectInfo[5]);
                    plantManager.AddPlant(new Plant(pos, objectInfo[4], growth));
                }
            }
            UpdateGPUVoxelData();
            UpdateGPUFenceData();

            float[] cubeVertices = Vertices.GetCubeVertices();

            cubeVertexBuffer = new VertexBuffer(cubeVertices, 6);
            cubeVertexArray = new VertexArray([3, 3], cubeVertexBuffer);

            plantVertexBuffer = new VertexBuffer(Vertices.GetPlantVertices(), 8);
            plantVertexArray = new VertexArray([3, 3, 2], plantVertexBuffer);
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

        public CubeTextureManager GetTextureManager()
        {
            return cubeTextureManager;
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
                fileSrc += voxel.GetVoxelType().ToString();
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

            foreach (Plant plant in plantManager.GetPlants())
            {
                fileSrc += "plant,";
                fileSrc += plant.GetPosition().X;
                fileSrc += ",";
                fileSrc += plant.GetPosition().Y;
                fileSrc += ",";
                fileSrc += plant.GetPosition().Z;
                fileSrc += ",";
                fileSrc += plant.GetPlantType().ToString();
                fileSrc += ",";
                fileSrc += plant.GetGrowth();
                fileSrc += "\n";
            }

            using (StreamWriter sw = new StreamWriter(filePath))
            {
                sw.Write(fileSrc);
            }
        }

        public void Render(Camera camera, Cursor? cursor = null)
        {
            cubeShader.Render(camera, cubeVertexArray, voxelsBuffers);
            cubeShader.Render(camera, cubeVertexArray, fenceBuffers);

            GL.Disable(EnableCap.CullFace);
            plantShader.Render(camera, plantVertexArray, plantManager.GetBuffers());
            GL.Enable(EnableCap.CullFace);

            if (cursor != null)
            {
                cubeShader.Render(camera, cubeVertexArray, cursor.GetShaderBuffers(), true);
            }
        }

        public void RenderCubeBufferSet(Camera camera, CubeShaderBufferSet bufferSet)
        {
            cubeShader.Render(camera, cubeVertexArray, bufferSet);
        }

        public void FrameUpdate(float deltaTime)
        {
            plantManager.UpdateGrowths(deltaTime);
        }

        /// <summary>
        /// Updates the GPU with the current scene data
        /// </summary>
        public void UpdateGPU(KeyboardState keyboard, MouseState mouse) // CALL THIS FUNCTION IF RETURN VALUE OF PLAYER.UPDATE IS TRUE
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
        /// Checks which plant is at a specific position, if any
        /// </summary>
        /// <returns>The plant sitting on top of a block at 'position', or null if there is no plant there</returns>
        public Plant? GetPlantOnVoxelAtPosition(Vector3 position)
        {
            foreach (Plant plant in plantManager.GetPlants())
            {
                // If positions are equal
                if (((plant.GetPosition() - Vector3.UnitY) - position).Length < 0.01)
                {
                    return plant;
                }
            }
            return null;
        }

        /// <summary>
        /// Checks which plant is on top of a block, if any
        /// </summary>
        /// <param name="voxel"></param>
        /// <returns>The plant sitting on top of the block, or null if there is no plant there</returns>
        public Plant? GetPlantOnVoxel(Voxel voxel)
        {
            return GetPlantOnVoxelAtPosition(voxel.GetPosition());
        }

        public void PlantSeed(Vector3 position, Plant.Type type)
        {
            plantManager.AddPlant(new Plant(position, type));
        }

        /// <summary>
        /// See plantManager.HarvestPlant(Plant)
        /// </summary>
        public int HarvestPlant(Plant plant)
        {
            return plantManager.HarvestPlant(plant);
        }

        /// <summary>
        /// Removes every log and leaf voxel touching a specific position recursivly
        /// </summary>
        /// <param name="position"></param>
        public void RemoveTree(Vector3 position)
        {
            Voxel? startingVoxel = GetVoxelAtPosition(position);
            if (startingVoxel != null)
            {
                if (startingVoxel.GetVoxelType() == Voxel.Type.log)
                {
                    List<Voxel> voxelsToRemove = new List<Voxel>();
                    voxelsToRemove.Add(startingVoxel);
                    while (voxelsToRemove.Count > 0)
                    {
                        Voxel currentVoxel = voxelsToRemove[0];
                        voxels.Remove(currentVoxel);
                        voxelsToRemove.AddRange(GetNeighbouringVoxels(currentVoxel, new List<Voxel.Type>() { Voxel.Type.log, Voxel.Type.leaves }));
                        voxelsToRemove.Remove(currentVoxel);
                    }
                }
            }
        }

        public List<Voxel> GetNeighbouringVoxels(Voxel startingVoxel, List<Voxel.Type>? typeMask)
        {
            List<Voxel> neighbours = new List<Voxel>();
            foreach (Voxel voxel in voxels)
            {
                if (MathF.Abs((startingVoxel.GetPosition() - voxel.GetPosition()).Length - 1) < 0.01f) {
                    if (typeMask != null)
                    {
                        if (typeMask.Contains(voxel.GetVoxelType()))
                        {
                            neighbours.Add(voxel);
                        }
                    }
                }
            }
            return neighbours;
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
                CubeShaderListSet listSet = voxels[i].GetGPUData(cubeTextureManager);
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
                CubeShaderListSet listSet;
                int cubeCount;
                (listSet, cubeCount) = fenceManager[i].GetGPUData(cubeTextureManager);
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
