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
        float growth = 0.0f; // 0 to 1
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
    }
}
