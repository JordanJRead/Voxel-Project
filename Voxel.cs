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
            none
        }

        public static readonly string[] typeNames = new string[(int)Type.none + 1] // Makes sure that the size of typeNames equals the number of Types
        {
            "grass",
            "stone",
            "none"
        };

        protected Vector3 position; // has to be an integer
        public Type type; // type has no invarient, so it can be public

        public Vector3 GetPosition()
        {
            return position;
        }

        public Voxel(Vector3 position, Type type)
        {
            this.position = position;
            this.type = type;
        }

        public Voxel(Vector3 position, string typeName)
        {
            this.position = position;
            this.type = VoxelTypeFromString(typeName);
        }

        public string GetTypeName()
        {
            return typeNames[(int)type];
        }

        private static Type VoxelTypeFromString(string name)
        {
            int index = Array.IndexOf(typeNames, name);
            if (index != -1)
            {
                return (Type)index;
            }
            return Type.none;
        }

        public override string ToString()
        {
            return $"Voxel( x = {position.X}, y = {position.Y}, z = {position.Z}, type = {GetTypeName()} = {(int)type})";
        }
    }
}
