using System;
using System.Collections.Generic;
using System.Formats.Tar;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Voxel_Project
{
    internal class Cursor
    {
        bool isVoxel = true;
        Voxel voxel;
        Fence fence;
        ShaderBufferSet bufferSet = new ShaderBufferSet();

        public Cursor(Vector3 position, Voxel.Type type, TextureManager textureManager)
        {
            voxel = new Voxel(position, type);
            fence = new Fence(position);
            UpdateGPUBuffers(textureManager);
        }

        public Vector3 GetPosition()
        {
            if (isVoxel)
                return voxel.GetPosition();
            return fence.GetPosition();
        }

        /// <summary>
        /// Updates the editor cursor with keyboard and mouse inputs
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="keyboard"></param>
        /// <returns>Whether the scene voxel data should be updated on the GPU</returns>
        public bool Update(Camera camera, KeyboardState keyboard, MouseState mouse, Scene scene)
        {
            bool hasSceneChanged = false;
            bool hasCursorChanged = false;

            TextureManager textureManager = scene.GetTextureManager();
            FenceManager fenceManager = scene.GetFenceManager();

            // Switch voxel/fence mode
            if (keyboard.IsKeyPressed(Keys.R))
            {
                isVoxel = !isVoxel;
                UpdateGPUBuffers(textureManager);
                if (isVoxel)
                {
                    fenceManager.UnFakeFence(voxel.GetPosition());
                }
                else
                {
                    fence = fenceManager.FakeFence(fence.GetPosition()); // fence.position and voxel.position are the same, I could use either one
                }
                hasSceneChanged = true;
                hasCursorChanged = true;
            }

            Vector3 prevPosition = voxel.GetPosition();
            if (keyboard.IsKeyDown(Keys.LeftControl))
            {
                hasCursorChanged = UpdatePosition(camera, keyboard) || hasCursorChanged;
                if (hasCursorChanged && !isVoxel)
                {
                    fenceManager.UnFakeFence(prevPosition);
                    fence = fenceManager.FakeFence(fence.GetPosition());
                    hasSceneChanged = true;
                } 

                // Cycle through cursor voxel types
                // Voxel.Type.none is the maximum enum value
                if (keyboard.IsKeyPressed(Keys.Q) && isVoxel) // Reverse
                {
                    int newType = (int)voxel.GetVoxelType() - 1;
                    if (newType < 0)
                    {
                        newType = (int)Voxel.Type.none;
                    }
                    voxel.SetType((Voxel.Type)(newType));
                    hasCursorChanged = true;
                }
                if (keyboard.IsKeyPressed(Keys.E) && isVoxel) // Forward
                {
                    int newType = (int)voxel.GetVoxelType() + 1;
                    if (newType > (int)Voxel.Type.none)
                    {
                        newType = 0;
                    }
                    voxel.SetType((Voxel.Type)(newType));
                    hasCursorChanged = true;
                }

                // Modifying voxels
                if (isVoxel)
                {
                    if (mouse.IsButtonPressed(MouseButton.Left))
                    {
                        Voxel? selectedVoxel = scene.GetVoxelAtPosition(this.voxel.GetPosition());

                        // Replacing voxel
                        if (selectedVoxel != null)
                        {
                            selectedVoxel.SetType(voxel.GetVoxelType());
                            hasSceneChanged = true;
                        }

                        // Place voxel
                        else
                        {
                            scene.AddVoxel(new Voxel(voxel.GetPosition(), voxel.GetVoxelType()));
                            hasSceneChanged = true;
                        }
                    }
                    // Deleting voxel
                    if (mouse.IsButtonPressed(MouseButton.Right))
                    {
                        Voxel? selectedVoxel = scene.GetVoxelAtPosition(this.voxel.GetPosition());

                        if (selectedVoxel != null)
                        {
                            scene.GetVoxels().Remove(selectedVoxel);
                            hasSceneChanged = true;
                        }
                        hasSceneChanged = true;
                    }
                }
                else if (!isVoxel)
                {
                    if (mouse.IsButtonPressed(MouseButton.Left))
                    {
                        Fence? selectedFence = fenceManager.GetFenceAtPosition(this.fence.GetPosition());

                        // Replacing fence
                        if (selectedFence != null)
                        {
                            //selectedVoxel.SetType(voxel.GetVoxelType());
                            //hasSceneChanged = true;
                        }

                        // Place fence
                        else
                        {
                            fenceManager.AddFence(this.fence.GetPosition());
                            hasSceneChanged = true;
                        }
                    }
                    // Deleting fence
                    if (mouse.IsButtonPressed(MouseButton.Right))
                    {
                        Fence? selectedFence = fenceManager.GetFenceAtPosition(this.fence.GetPosition());

                        if (selectedFence != null)
                        {
                            fenceManager.RemoveFence(selectedFence);
                            fence = fenceManager.FakeFence(fence.GetPosition());
                            hasSceneChanged = true;
                        }
                    }
                }
            }
            if (hasCursorChanged)
            {
                UpdateGPUBuffers(textureManager);
            }
            return hasSceneChanged;
        }

        /// <summary>
        /// Moves the cursor with keyboard input
        /// </summary>
        /// <returns>Whether the cursor moved or not</returns>
        private bool UpdatePosition(Camera camera, KeyboardState keyboard)
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
                voxel.MoveBy(cursorForwardAxis);
                fence.MoveBy(cursorForwardAxis);
                return true;
            }
            if (keyboard.IsKeyPressed(Keys.S))
            {
                voxel.MoveBy(-cursorForwardAxis);
                fence.MoveBy(-cursorForwardAxis);
                return true;
            }
            if (keyboard.IsKeyPressed(Keys.A))
            {
                voxel.MoveBy(-rightAxis);
                fence.MoveBy(-rightAxis);
                return true;
            }
            if (keyboard.IsKeyPressed(Keys.D))
            {
                voxel.MoveBy(rightAxis);
                fence.MoveBy(rightAxis);
                return true;
            }
            if (keyboard.IsKeyPressed(Keys.Space))
            {
                voxel.MoveBy(Vector3.UnitY);
                fence.MoveBy(Vector3.UnitY);
                return true;
            }
            if (keyboard.IsKeyPressed(Keys.LeftShift))
            {
                voxel.MoveBy(-Vector3.UnitY);
                fence.MoveBy(-Vector3.UnitY);
                return true;
            }
            return false;
        }

        private void SwitchType()
        {
            isVoxel = !isVoxel;
        }

        private void UpdateGPUBuffers(TextureManager textureManager)
        {
            ShaderListSet listSet;
            int cubeCount;
            if (isVoxel)
            {
                listSet = voxel.GetGPUData(textureManager);
                cubeCount = 1;
            }
            else
            {
                (listSet, cubeCount) = fence.GetGPUData(textureManager);
            }
            bufferSet.SetPositions(listSet.positions);
            bufferSet.SetScales(listSet.scales);
            bufferSet.SetTextureHandles(listSet.textureHandles);
        }

        public ShaderBufferSet GetShaderBuffers()
        {
            return bufferSet;
        }
    }
}
