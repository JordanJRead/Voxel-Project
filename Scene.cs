using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Voxel_Project;
using Voxel_Project.OpenGL_Objects;

namespace Voxel_Project
{
    internal class Scene
    {
        List<Voxel> voxels = new List<Voxel>();
        FenceManager fenceManager = new FenceManager();

        CloudManager cloudManager = new CloudManager();

        // Cube vertices
        VertexArray cubeVertexArray;
        VertexBuffer cubeVertexBuffer;

        // Plant vertices
        VertexArray plantVertexArray;
        VertexBuffer plantVertexBuffer;

        PlantManager plantManager = new PlantManager();

        ScreenTextureShader screenTextureShader = new ScreenTextureShader("Shaders/screentexture.vert", "Shaders/screentexture.frag");

        CubeShader cubeShader = new CubeShader("Shaders/cube.vert", "Shaders/cube.frag");
        PlantShader plantShader = new PlantShader("Shaders/plant.vert", "Shaders/plant.frag");
        CelestialShader celestialShader = new CelestialShader("Shaders/celeste.vert", "Shaders/celeste.frag");
        float dayProgress = 0;
        float time = 0;
        const float secondsPerDayCycle = 240;

        DepthCubeShader depthCubeShader = new DepthCubeShader("shaders/Depth/depthcube.vert", "shaders/Depth/depthcube.frag");
        DepthPlantShader depthPlantShader = new DepthPlantShader("shaders/Depth/depthplant.vert", "shaders/Depth/depthplant.frag");

        CubeShaderBufferSet voxelsBuffers = new CubeShaderBufferSet();
        CubeShaderBufferSet fenceBuffers = new CubeShaderBufferSet();

        CubeTextureManager cubeTextureManager = new CubeTextureManager();
        string initialPath;

        ShadowMapper sunShadowMapper;
        ShadowMapper moonShadowMapper;

        /// <summary>
        /// Loads scene data from a file path into program memory
        /// </summary>
        public Scene(string filePath, int screenWidth, int screenHeight)
        {
            sunShadowMapper = new ShadowMapper(screenWidth, screenHeight);
            moonShadowMapper = new ShadowMapper(screenWidth, screenHeight, false);

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

        public void AddFence(Fence fence)
        {
            fenceManager.AddFence(fence);
        }

        public FenceManager GetFenceManager()
        {
            return fenceManager;
        }

        public CubeTextureManager GetTextureManager()
        {
            return cubeTextureManager;
        }

        public float GetDayProgress()
        {
            return dayProgress;
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

        public void Render(ICamera camera, Cursor? cursor = null)
        {
            cubeShader.Render(camera, cubeVertexArray, voxelsBuffers, dayProgress, sunShadowMapper, moonShadowMapper);
            cubeShader.Render(camera, cubeVertexArray, fenceBuffers, dayProgress, sunShadowMapper, moonShadowMapper);
            cubeShader.Render(camera, cubeVertexArray, cloudManager.GetBufferSet(), dayProgress, sunShadowMapper, moonShadowMapper, false, true);
            celestialShader.Render(camera, cubeVertexArray, dayProgress);

            plantShader.Render(camera, plantVertexArray, plantManager.GetBuffers(), dayProgress, time, sunShadowMapper);

            if (cursor != null)
            {
                cubeShader.Render(camera, cubeVertexArray, cursor.GetShaderBuffers(), dayProgress, sunShadowMapper, moonShadowMapper, true);
            }

            //screenTextureShader.Render(moonShadowMapper.GetDepthTexture());
        }


        /// <summary>
        /// Renders only the depth information of the scene (used for shadow mapping)
        /// </summary>
        public void RenderDepth(ICamera camera)
        {
            depthCubeShader.Render(camera, cubeVertexArray, voxelsBuffers);
            depthCubeShader.Render(camera, cubeVertexArray, fenceBuffers);
            depthPlantShader.Render(camera, plantVertexArray, plantManager.GetBuffers());
        }

        public void RenderCubeBufferSet(Camera camera, CubeShaderBufferSet bufferSet)
        {
            cubeShader.Render(camera, cubeVertexArray, bufferSet, dayProgress, sunShadowMapper, moonShadowMapper);
        }

        public void RenderTextureToScreen(Texture2D tex)
        {
            screenTextureShader.Render(tex);
        }

        public void FrameUpdate(float deltaTime)
        {
            plantManager.UpdateGrowths(deltaTime);
            cloudManager.MoveClouds(deltaTime);
            time += deltaTime;
            dayProgress += deltaTime / secondsPerDayCycle;
            if (dayProgress > 1)
            {
                dayProgress -= 1;
            }
            GL.ClearColor(0, 0, DayStrength(), 1);
            sunShadowMapper.UpdateShadows(this);
            moonShadowMapper.UpdateShadows(this);
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

        public bool IsPositionEmpty(Vector3 position)
        {
            foreach (Voxel voxel in voxels)
            {
                if (MathF.Abs((voxel.GetPosition() - position).Length) < 0.01f)
                    return false;
            }
            foreach (Fence fence in fenceManager.GetFences())
            {
                if (MathF.Abs((fence.GetPosition() - position).Length) < 0.01f)
                    return false;
            }
            foreach (Plant plant in plantManager.GetPlants())
            {
                if (MathF.Abs((plant.GetPosition() - position).Length) < 0.01f)
                    return false;
            }
            return true;
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

        /// <summary>
        /// Old version of function:
        /// https://www.desmos.com/calculator/pj2o231y4i
        /// New version is just a sine wave
        /// 
        /// ...
        /// Gives the 'strenght' of day based on the dayProgress. It is 1 during noon, 0 during midnight, and stays at 1/0 for a good amount of time around those times.
        /// Another way to think of it is the blueness of the sky color, where it is blue during the day, black at night, and smoothly transitions during certain times
        /// 
        /// Looks like:
        /// 
        /// 
        /// 
        /// -------------                              ---------------
        ///              \                            /
        ///               \                          /
        ///                \                        /
        ///                 ------------------------
        /// 
        /// 
        /// </summary>
        /// <returns></returns>
        public float DayStrength()
        {
            return MathF.Sin(dayProgress * 2 * MathF.PI) / 2.0f + 0.5f;
            float transitionTime = 0.5f;
            
            // Smoothing function
            float Smooth(float x)
            {
                if (x < 0.5)
                {
                    return 4 * x * x * x;
                }
                else
                {
                    return 1 - (-2 * x + 2) * (-2 * x + 2) * (-2 * x + 2) / 2.0f;
                }
            }

            if (0 < dayProgress && dayProgress < transitionTime / 2)
            {
                return Smooth((dayProgress + transitionTime / 2) / transitionTime);
            }
            else if (dayProgress < 0.5 - transitionTime / 2)
            {
                return 1;
            }
            else if (dayProgress < 0.5 + transitionTime / 2)
            {
                return Smooth(1 - (dayProgress - (0.5f - transitionTime / 2)) / transitionTime);
            }
            else if (dayProgress < 1 - transitionTime / 2)
            {
                return 0;
            }
            else
            {
                return Smooth((dayProgress - (1 - transitionTime / 2)) / transitionTime);
            }
        }
    }
}
