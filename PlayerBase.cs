using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Voxel_Project
{
    internal abstract class PlayerBase
    {
        protected Camera camera;
        protected float speed;

        protected PlayerBase(Camera camera)
        {
            this.camera = camera;
        }

        public abstract void Update(MouseState mouse, KeyboardState keyboard, float deltaTime, Scene scene);

        public Camera GetCamera()
        {
            return camera;
        }
    }
}
