using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Voxel_Project
{
    /// <summary>
    /// Controls how the user interacts with the world in editor mode
    /// </summary>
    internal class EditorController : ControllerBase
    {
        Cursor cursor; // The transparent voxel / fence that can be moved around in editor mode
        CubeShaderBufferSet playerCubeBufferSet = new CubeShaderBufferSet(); // For rendering the player controller

        public EditorController(Camera camera, CubeTextureManager cubeTextureManager) : base(camera)
        {
            this.speed = 5;
            cursor = new Cursor(new Vector3(0, 0, 0), Voxel.Type.none, cubeTextureManager); // The transparent voxel that can be moved around in editor mode
        }

        public Cursor GetCursor() { return cursor; }

        /// <summary>
        /// Sets the player controller's buffer set so they can start being rendered
        /// </summary>
        public void Activate(PlayerController playerController, CubeTextureManager cubeTextureManager)
        {
            List<float> position = new List<float>(3);
            position.Add(playerController.GetPosition().X);
            position.Add(playerController.GetPosition().Y);
            position.Add(playerController.GetPosition().Z);

            List<float> scale = new List<float>(3);
            scale.Add(playerController.GetSize().X);
            scale.Add(playerController.GetSize().Y);
            scale.Add(playerController.GetSize().Z);

            List<ulong> texture = new List<ulong>();
            texture.Add(cubeTextureManager.GetBindlessTextureHandle(Voxel.Type.none));

            playerCubeBufferSet.SetPositions(position);
            playerCubeBufferSet.SetScales(scale);
            playerCubeBufferSet.SetTextureHandles(texture);
        }

        /// <summary>
        /// Gets the buffer set representing the player controller
        /// </summary>
        public CubeShaderBufferSet GetPlayerCubeBufferSet()
        {
            return playerCubeBufferSet;
        }

        public override bool Update(MouseState mouse, KeyboardState keyboard, float deltaTime, Scene scene)
        {
            if (!keyboard.IsKeyDown(Keys.LeftControl))
            {
                Vector3 moveVector = new Vector3();

                Vector3 forward = camera.GetForward();
                Vector3 right = camera.GetRight();

                if (keyboard.IsKeyDown(Keys.Space))
                    moveVector += Vector3.UnitY;
                if (keyboard.IsKeyDown(Keys.LeftShift))
                    moveVector -= Vector3.UnitY;
                if (keyboard.IsKeyDown(Keys.W))
                    moveVector += forward;
                if (keyboard.IsKeyDown(Keys.S))
                    moveVector -= forward;
                if (keyboard.IsKeyDown(Keys.A))
                    moveVector -= right;
                if (keyboard.IsKeyDown(Keys.D))
                    moveVector += right;
                
                camera.SetPosition(moveVector * deltaTime * speed + camera.GetPosition());

                if (mouse.IsButtonPressed(MouseButton.Left))
                {
                    Voxel? voxel = PhysicsManager.RayTraceVoxel(camera.GetPosition(), camera.GetForward(), 5, scene);
                    Console.WriteLine(voxel);
                }
            }
            camera.Update(mouse, keyboard);
            return cursor.Update(camera, keyboard, mouse, scene);
        }
    }
}
