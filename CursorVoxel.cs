using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Voxel_Project
{
    internal class CursorVoxel : Voxel
    {
        Voxel voxel;

        public CursorVoxel(Vector3 position, Voxel.Type type) : base(position, type) { }

        /// <summary>
        /// Updates the editor cursor with keyboard and mouse inputs
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="keyboard"></param>
        /// <returns>Whether the scene voxel data should be updated on the GPU</returns>
        public bool Update(Camera camera, KeyboardState keyboard, MouseState mouse, Scene scene)
        {
            bool hasSceneChanged = false;
            if (keyboard.IsKeyDown(Keys.LeftControl))
            {
                Vector3 cameraForward = camera.GetForward();
                Vector3 cursorForwardAxis = new Vector3(1, 0, 0);

                int[] axisValues = { -1, 1 };

                Vector3 currentAxis;

                // Find the best axis to move the cube along
                // The dot product returns a greater value the more aligned the vectors are
                currentAxis = Vector3.UnitX;
                if (Vector3.Dot(cameraForward, currentAxis) > Vector3.Dot(cameraForward, cursorForwardAxis))
                {
                    cursorForwardAxis = currentAxis;
                }
                currentAxis = -Vector3.UnitX;
                if (Vector3.Dot(cameraForward, currentAxis) > Vector3.Dot(cameraForward, cursorForwardAxis))
                {
                    cursorForwardAxis = currentAxis;
                }
                currentAxis = Vector3.UnitZ;
                if (Vector3.Dot(cameraForward, currentAxis) > Vector3.Dot(cameraForward, cursorForwardAxis))
                {
                    cursorForwardAxis = currentAxis;
                }
                currentAxis = -Vector3.UnitZ;
                if (Vector3.Dot(cameraForward, currentAxis) > Vector3.Dot(cameraForward, cursorForwardAxis))
                {
                    cursorForwardAxis = currentAxis;
                }

                Vector3 rightAxis = Vector3.Cross(cursorForwardAxis, Vector3.UnitY);

                // Move cursor
                if (keyboard.IsKeyPressed(Keys.W))
                {
                    position += cursorForwardAxis;
                }
                if (keyboard.IsKeyPressed(Keys.S))
                {
                    position -= cursorForwardAxis;
                }
                if (keyboard.IsKeyPressed(Keys.A))
                {
                    position -= rightAxis;
                }
                if (keyboard.IsKeyPressed(Keys.D))
                {
                    position += rightAxis;
                }
                if (keyboard.IsKeyPressed(Keys.Space))
                {
                    position += Vector3.UnitY;
                }
                if (keyboard.IsKeyPressed(Keys.LeftShift))
                {
                    position -= Vector3.UnitY;
                }

                // Cycle through cursor voxel types
                // Voxel.Type.none is the maximum enum value
                if (keyboard.IsKeyPressed(Keys.Q)) // Reverse
                {
                    int newType = (int)type - 1;
                    if (newType < 0)
                    {
                        newType = (int)Voxel.Type.none;
                    }
                    type = (Voxel.Type)(newType);
                }
                if (keyboard.IsKeyPressed(Keys.E)) // Forward
                {
                    int newType = (int)type + 1;
                    if (newType > (int)Voxel.Type.none)
                    {
                        newType = 0;
                    }
                    type = (Voxel.Type)(newType);
                }

                // Modifying voxels
                Voxel? selectedVoxel = scene.GetSelectedVoxel();
                if (selectedVoxel != null)
                {
                    // Replacing voxel
                    if (mouse.IsButtonPressed(MouseButton.Left))
                    {
                        selectedVoxel.type = type;
                        hasSceneChanged = true;
                    }
                    // Deleting voxel
                    if (mouse.IsButtonPressed(MouseButton.Right))
                    {
                        scene.GetVoxels().Remove(selectedVoxel);
                        hasSceneChanged = true;
                    }
                }
                else
                {
                    // Placing voxel
                    if (mouse.IsButtonPressed(MouseButton.Left))
                    {
                        scene.AddVoxel(new Voxel(position, type));
                        hasSceneChanged = true;
                    }
                }
            }
            return hasSceneChanged;
        }
    }
}
