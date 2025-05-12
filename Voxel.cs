using System;
using System.Collections.Generic;
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
        public Voxel(Vector3 position, Type type)
        {
            this.position = position;
            this.type = type;
        }

        public enum Type
        {
            none,
            grass,
            stone
        }

        Vector3 position;
        Type type;

        public static Type VoxelTypeFromString(string name)
        {
            switch (name)
            {
                case "grass":
                    return Type.grass;
                case "stone":
                    return Type.grass;
            }
            return Type.none;
        }
    }
}
