using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Voxel_Project
{
    internal class Plant
    {
        public enum Type
        {
            wheat,
            strawberries,
            blueberries,

            none
        }

        Vector3 position;
        float growth = 0.0f; // 0 - 1
        Type type;

        public Plant(Vector3 position, Type type, float growth = 0)
        {
            this.growth = growth;
            this.position = position;
            this.type = type;
        }

        public Plant(Vector3 position, string typeName, float growth = 0)
        {
            this.growth = growth;
            this.position = position;
            this.type = Enum.Parse<Type>(typeName);
        }

        public Vector3 GetPosition()
        {
            return position;
        }

        public float GetGrowth()
        {
            return growth;
        }

        public void GrowBy(float grow)
        {
            growth += grow;
        }

        public void SetGrowth(float grow)
        {
            growth = grow;
        }

        public Type GetPlantType()
        {
            return type;
        }

        public void SetType(Type type)
        {
            this.type = type;
        }

        public CubeShaderListSet GetGPUData(PlantTextxureManager plantTextureManager)
        {
            CubeShaderListSet listSet = new CubeShaderListSet();

            // POSITIONS (the center of the cube the voxel is in, may not look like the center if the plant is not fully grown)
            // Position data is stored as x1, y1, z1, x2, y2, z2...
            // because vec3 is not memory compact with SSBOs
            // and there may be differences in the memory layout between CPU and GPU
            listSet.positions.Add(position.X);
            listSet.positions.Add(position.Y);
            listSet.positions.Add(position.Z);

            // SCALES
            // Scale data is stored as x1, y1, z1, x2, y2, z2...
            listSet.scales.Add(1);
            listSet.scales.Add(1);
            listSet.scales.Add(1);

            listSet.textureHandles.Add((ulong)plantTextureManager.GetBindlessTextureHandle(type));

            return listSet;
        }
    }
}
