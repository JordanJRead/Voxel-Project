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

        float[] plantGrowTimes = new float[(int)Plant.Type.none + 1]
        {
            10,
            10,
            10
        };

        public PlantManager()
        {
        }

        public void AddPlant(Plant plant)
        {
            plants.Add(plant);
        }

        public void RemovePlant(Plant plant)
        {
            plants.Remove(plant);
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
