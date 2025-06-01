using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Voxel_Project
{
    internal class PlantManager
    {
        List<Plant> plants = new List<Plant>();
        PlantShaderBufferSet bufferSet = new PlantShaderBufferSet();
        PlantTextxureManager plantTextureManager = new PlantTextxureManager();

        float[] plantGrowTimes;
        int[] plantHarvestMoney;

        public PlantManager()
        {
            plantGrowTimes = new float[(int)Plant.Type.none + 1]
            {
                10,
                10,
                10
            };

            plantHarvestMoney = new int[(int)Plant.Type.none + 1]
            {
                10,
                15,
                20
            };
        }

        public void AddPlant(Plant plant)
        {
            plants.Add(plant);
        }

        /// <summary>
        /// Removes a plant from the scene
        /// </summary>
        /// <returns>The money the player gets from harvesting the plant</returns>
        public int HarvestPlant(Plant plant)
        {
            plants.Remove(plant);
            return plantHarvestMoney[(int)plant.GetPlantType()];
        }

        public PlantShaderBufferSet GetBuffers()
        {
            return bufferSet;
        }

        public List<Plant> GetPlants()
        {
            return plants;
        }

        /// <summary>
        /// Grows plants. Should be called every frame
        /// </summary>
        public void UpdateGrowths(float deltaTime)
        {
            foreach (Plant plant in plants)
            {
                plant.GrowBy(deltaTime /  plantGrowTimes[(int)plant.GetPlantType()]);
                //Console.WriteLine(plant.GetGrowth());
                if (plant.GetGrowth() > 1)
                {
                    plant.SetGrowth(1);
                }
            }
            UpdateGPUInfo();
        }

        public void UpdateGPUInfo()
        {
            PlantShaderListSet listSet = new PlantShaderListSet();

            foreach (Plant plant in plants)
            {
                listSet.positions.Add(plant.GetPosition().X);
                listSet.positions.Add(plant.GetPosition().Y);
                listSet.positions.Add(plant.GetPosition().Z);

                listSet.growths.Add(plant.GetGrowth());

                listSet.textureHandles.Add(plantTextureManager.GetBindlessTextureHandle(plant.GetPlantType()));
            }

            bufferSet.SetFromListSet(listSet);
        }
    }
}
