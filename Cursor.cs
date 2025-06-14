﻿using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Formats.Tar;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace Voxel_Project
{
    /// <summary>
    /// The edito's cursor that can be used to manipulate the scene
    /// </summary>
    internal class Cursor
    {
        bool isVoxel = true;
        Voxel voxel;
        Fence fence;
        CubeShaderBufferSet bufferSet = new CubeShaderBufferSet(); // For rendering the cursor

        // Selection volume
        Vector3 selectPos1;
        Vector3 selectPos2;
        List<Voxel> copiedVoxels = new List<Voxel>();

        public Cursor(Vector3 position, Voxel.Type type, CubeTextureManager cubeTextureManager)
        {
            voxel = new Voxel(position, type);
            fence = new Fence(position);
            UpdateGPUBuffers(cubeTextureManager);
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
        /// <returns>Whether the scene data should be updated on the GPU</returns>
        public bool Update(Camera camera, KeyboardState keyboard, MouseState mouse, Scene scene)
        {
            bool hasSceneChanged = false;
            bool hasCursorChanged = false;

            CubeTextureManager cubeTextureManager = scene.GetTextureManager();
            FenceManager fenceManager = scene.GetFenceManager();

            Vector3 prevPosition = voxel.GetPosition();
            if (keyboard.IsKeyDown(Keys.LeftControl))
            {
                // Switch voxel/fence mode
                if (keyboard.IsKeyPressed(Keys.R))
                {
                    isVoxel = !isVoxel;
                    UpdateGPUBuffers(cubeTextureManager);
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

                hasCursorChanged = UpdatePosition(camera, keyboard) || hasCursorChanged;

                // Update fence preview information
                if (hasCursorChanged && !isVoxel)
                {
                    fenceManager.UnFakeFence(prevPosition);
                    fence = fenceManager.FakeFence(fence.GetPosition());
                    hasSceneChanged = true;
                }

                Voxel? selectedVoxel = scene.GetVoxelAtPosition(this.voxel.GetPosition());

                // Trees
                if (keyboard.IsKeyPressed(Keys.T))
                {
                    // Delete tree
                    if (selectedVoxel != null)
                    {
                        if (selectedVoxel.GetVoxelType() == Voxel.Type.log)
                        {
                            scene.RemoveTree(this.voxel.GetPosition());
                            hasSceneChanged = true;
                        }
                    }

                    // Build a tree
                    else
                    {
                        hasSceneChanged = true;
                        // Trunk
                        scene.AddVoxel(new Voxel(this.voxel.GetPosition() + Vector3.UnitY * 0, Voxel.Type.log));
                        scene.AddVoxel(new Voxel(this.voxel.GetPosition() + Vector3.UnitY * 1, Voxel.Type.log));
                        scene.AddVoxel(new Voxel(this.voxel.GetPosition() + Vector3.UnitY * 2, Voxel.Type.log));

                        // Leaves layer 1
                        scene.AddVoxel(new Voxel(this.voxel.GetPosition() + Vector3.UnitY * 2 + Vector3.UnitX * 1, Voxel.Type.leaves));
                        scene.AddVoxel(new Voxel(this.voxel.GetPosition() + Vector3.UnitY * 2 + Vector3.UnitX * 2, Voxel.Type.leaves));
                        scene.AddVoxel(new Voxel(this.voxel.GetPosition() + Vector3.UnitY * 2 + Vector3.UnitX * -1, Voxel.Type.leaves));
                        scene.AddVoxel(new Voxel(this.voxel.GetPosition() + Vector3.UnitY * 2 + Vector3.UnitX * -2, Voxel.Type.leaves));
                        scene.AddVoxel(new Voxel(this.voxel.GetPosition() + Vector3.UnitY * 2 + Vector3.UnitZ * 1, Voxel.Type.leaves));
                        scene.AddVoxel(new Voxel(this.voxel.GetPosition() + Vector3.UnitY * 2 + Vector3.UnitZ * 2, Voxel.Type.leaves));
                        scene.AddVoxel(new Voxel(this.voxel.GetPosition() + Vector3.UnitY * 2 + Vector3.UnitZ * -1, Voxel.Type.leaves));
                        scene.AddVoxel(new Voxel(this.voxel.GetPosition() + Vector3.UnitY * 2 + Vector3.UnitZ * -2, Voxel.Type.leaves));
                        scene.AddVoxel(new Voxel(this.voxel.GetPosition() + Vector3.UnitY * 2 + Vector3.UnitX + Vector3.UnitZ, Voxel.Type.leaves));
                        scene.AddVoxel(new Voxel(this.voxel.GetPosition() + Vector3.UnitY * 2 + Vector3.UnitX - Vector3.UnitZ, Voxel.Type.leaves));
                        scene.AddVoxel(new Voxel(this.voxel.GetPosition() + Vector3.UnitY * 2 - Vector3.UnitX + Vector3.UnitZ, Voxel.Type.leaves));
                        scene.AddVoxel(new Voxel(this.voxel.GetPosition() + Vector3.UnitY * 2 - Vector3.UnitX - Vector3.UnitZ, Voxel.Type.leaves));

                        // Leaves layer 2
                        scene.AddVoxel(new Voxel(this.voxel.GetPosition() + Vector3.UnitY * 3, Voxel.Type.leaves));
                        scene.AddVoxel(new Voxel(this.voxel.GetPosition() + Vector3.UnitY * 3 + Vector3.UnitX * 1, Voxel.Type.leaves));
                        scene.AddVoxel(new Voxel(this.voxel.GetPosition() + Vector3.UnitY * 3 + Vector3.UnitX * -1, Voxel.Type.leaves));
                        scene.AddVoxel(new Voxel(this.voxel.GetPosition() + Vector3.UnitY * 3 + Vector3.UnitZ * 1, Voxel.Type.leaves));
                        scene.AddVoxel(new Voxel(this.voxel.GetPosition() + Vector3.UnitY * 3 + Vector3.UnitZ * -1, Voxel.Type.leaves));

                        scene.AddVoxel(new Voxel(this.voxel.GetPosition() + Vector3.UnitY * 4, Voxel.Type.leaves));
                    }
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
                        // Replacing voxel
                        if (selectedVoxel != null)
                        {
                            selectedVoxel.SetType(voxel.GetVoxelType());
                            hasSceneChanged = true;
                        }

                        // Placing voxel
                        else
                        {
                            scene.AddVoxel(new Voxel(voxel.GetPosition(), voxel.GetVoxelType()));
                            hasSceneChanged = true;
                        }
                    }
                    // Deleting voxel
                    if (mouse.IsButtonPressed(MouseButton.Right))
                    {
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

                        // Replacing fence (empty because there is only one fence type)
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

                // Volume selection
                if (keyboard.IsKeyPressed(Keys.Y))
                {
                    selectPos1 = voxel.GetPosition();
                }
                if (keyboard.IsKeyPressed(Keys.U))
                {
                    selectPos2 = voxel.GetPosition();
                }

                // Fill
                if (keyboard.IsKeyPressed(Keys.F) && isVoxel)
                {
                    hasSceneChanged = true;
                    DeleteSelectedVolume(scene);
                    FillSelectedVolume(scene);
                }

                // Delete
                if (keyboard.IsKeyPressed(Keys.G) && isVoxel)
                {
                    hasSceneChanged = true;
                    DeleteSelectedVolume(scene);
                }

                // Copy
                if (keyboard.IsKeyPressed(Keys.C))
                {
                    CopySelectedVolume(scene);
                }

                // Paste
                if (keyboard.IsKeyPressed(Keys.V))
                {
                    PasteCopiedVoxels(scene);
                    hasSceneChanged = true;
                }

                // Rotate
                if (keyboard.IsKeyPressed(Keys.B))
                {
                    RotateCopiedVoxels();
                    hasSceneChanged = true;
                }
            }
            if (hasCursorChanged)
            {
                UpdateGPUBuffers(cubeTextureManager);
            }
            return hasSceneChanged;
        }

        /// <summary>
        /// Deletes all the voxels in the selected volume
        /// </summary>
        private void DeleteSelectedVolume(Scene scene)
        {
            Vector3 minPos = new Vector3(MathF.Min(selectPos1.X, selectPos2.X), MathF.Min(selectPos1.Y, selectPos2.Y), MathF.Min(selectPos1.Z, selectPos2.Z));
            Vector3 maxPos = new Vector3(MathF.Max(selectPos1.X, selectPos2.X), MathF.Max(selectPos1.Y, selectPos2.Y), MathF.Max(selectPos1.Z, selectPos2.Z));

            for (float x = minPos.X; x <= maxPos.X; ++x)
            {
                for (float y = minPos.Y; y <= maxPos.Y; ++y)
                {
                    for (float z = minPos.Z; z <= maxPos.Z; ++z)
                    {
                        scene.RemoveVoxelAtPosition(new Vector3(x, y, z));
                    }
                }
            }
        }

        /// <summary>
        /// Fills the selected volume with the selected voxel type
        /// </summary>
        /// <param name="scene"></param>
        private void FillSelectedVolume(Scene scene)
        {
            Vector3 minPos = new Vector3(MathF.Min(selectPos1.X, selectPos2.X), MathF.Min(selectPos1.Y, selectPos2.Y), MathF.Min(selectPos1.Z, selectPos2.Z));
            Vector3 maxPos = new Vector3(MathF.Max(selectPos1.X, selectPos2.X), MathF.Max(selectPos1.Y, selectPos2.Y), MathF.Max(selectPos1.Z, selectPos2.Z));

            for (float x = minPos.X; x <= maxPos.X; ++x)
            {
                for (float y = minPos.Y; y <= maxPos.Y; ++y)
                {
                    for (float z = minPos.Z; z <= maxPos.Z; ++z)
                    {
                        scene.AddVoxel(new Voxel(new Vector3(x, y, z), this.voxel.GetVoxelType()));
                    }
                }
            }
        }

        private void CopySelectedVolume(Scene scene)
        {
            copiedVoxels.Clear();
            Vector3 minPos = new Vector3(MathF.Min(selectPos1.X, selectPos2.X), MathF.Min(selectPos1.Y, selectPos2.Y), MathF.Min(selectPos1.Z, selectPos2.Z));
            Vector3 maxPos = new Vector3(MathF.Max(selectPos1.X, selectPos2.X), MathF.Max(selectPos1.Y, selectPos2.Y), MathF.Max(selectPos1.Z, selectPos2.Z));

            for (float x = minPos.X; x <= maxPos.X; ++x)
            {
                for (float y = minPos.Y; y <= maxPos.Y; ++y)
                {
                    for (float z = minPos.Z; z <= maxPos.Z; ++z)
                    {
                        Voxel? voxel = scene.GetVoxelAtPosition(new Vector3(x, y, z));
                        if (voxel != null)
                        {
                            copiedVoxels.Add(new Voxel(voxel.GetPosition() - selectPos1, voxel.GetVoxelType()));
                        }
                    }
                }
            }
        }

        private void RotateCopiedVoxels()
        {
            foreach (Voxel voxel in copiedVoxels)
            {
                Vector3 oldPosition = voxel.GetPosition();
                Vector3 newPosition = new Vector3(oldPosition.Z, oldPosition.Y, -oldPosition.X);
                voxel.SetPosition(newPosition);
            }
        }

        private void PasteCopiedVoxels(Scene scene)
        {
            foreach (Voxel voxel in copiedVoxels)
            {
                Vector3 worldPos = voxel.GetPosition() + this.voxel.GetPosition();
                scene.RemoveVoxelAtPosition(worldPos);
                scene.AddVoxel(new Voxel(worldPos, voxel.GetVoxelType()));
            }
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

            float moveScale = keyboard.IsKeyDown(Keys.LeftAlt) ? 5 : 1;

            // Move cursor
            if (keyboard.IsKeyPressed(Keys.W))
            {
                voxel.MoveBy(cursorForwardAxis * moveScale);
                fence.MoveBy(cursorForwardAxis * moveScale);
                return true;
            }
            if (keyboard.IsKeyPressed(Keys.S))
            {
                voxel.MoveBy(-cursorForwardAxis * moveScale);
                fence.MoveBy(-cursorForwardAxis * moveScale);
                return true;
            }
            if (keyboard.IsKeyPressed(Keys.A))
            {
                voxel.MoveBy(-rightAxis * moveScale);
                fence.MoveBy(-rightAxis * moveScale);
                return true;
            }
            if (keyboard.IsKeyPressed(Keys.D))
            {
                voxel.MoveBy(rightAxis * moveScale);
                fence.MoveBy(rightAxis * moveScale);
                return true;
            }
            if (keyboard.IsKeyPressed(Keys.Space))
            {
                voxel.MoveBy(Vector3.UnitY * moveScale);
                fence.MoveBy(Vector3.UnitY * moveScale);
                return true;
            }
            if (keyboard.IsKeyPressed(Keys.LeftShift))
            {
                voxel.MoveBy(-Vector3.UnitY * moveScale);
                fence.MoveBy(-Vector3.UnitY * moveScale);
                return true;
            }
            return false;
        }

        private void SwitchType()
        {
            isVoxel = !isVoxel;
        }

        private void UpdateGPUBuffers(CubeTextureManager cubeTextureManager)
        {
            CubeShaderListSet listSet;
            int cubeCount;
            if (isVoxel)
            {
                listSet = voxel.GetGPUData(cubeTextureManager);
                cubeCount = 1;
            }
            else
            {
                (listSet, cubeCount) = fence.GetGPUData(cubeTextureManager);
            }
            bufferSet.SetPositions(listSet.positions);
            bufferSet.SetScales(listSet.scales);
            bufferSet.SetTextureHandles(listSet.textureHandles);
        }

        public CubeShaderBufferSet GetShaderBuffers()
        {
            return bufferSet;
        }
    }
}
