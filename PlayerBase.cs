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

        /// <summary>
        /// Returns true if the scene was changed because of the update
        /// </summary>
        public abstract bool Update(MouseState mouse, KeyboardState keyboard, float deltaTime, Scene scene);

        public Camera GetCamera()
        {
            return camera;
        }
    }
}
