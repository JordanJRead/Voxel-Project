using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Voxel_Project
{
    internal class Voxel
    {
        public enum Type
        {
            grass,
            stone,
            tilled,
            log,
            leaves,

            none
        }

        protected Vector3 position; // has to be an integer
        public Type type; // type has no invarient, so it can be public

        public Vector3 GetPosition()
        {
            return position;
        }

        public void MoveBy(Vector3 moveBy)
        {
            position += moveBy;
        }

        public Voxel(Vector3 position, Type type)
        {
            this.position = position;
            this.type = type;
        }

        public Voxel(Vector3 position, string typeName)
        {
            this.position = position;
            this.type = Enum.Parse<Type>(typeName);
        }

        public Type GetVoxelType()
        {
            return type;
        }

        public void SetType(Type type)
        {
            this.type = type;
        }


        public override string ToString()
        {
            return $"Voxel( x = {position.X}, y = {position.Y}, z = {position.Z}, type = {type.ToString()} = {(int)type})";
        }

        public CubeShaderListSet GetGPUData(CubeTextureManager cubeTextureManager)
        {
            CubeShaderListSet listSet = new CubeShaderListSet();

            // POSITIONS
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

            listSet.textureHandles.Add((ulong)cubeTextureManager.GetBindlessTextureHandle(type));

            return listSet;
        }
    }
}
