using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Voxel_Project
{
    internal class EditorController : ControllerBase
    {
        Cursor cursor; // The transparent voxel / fence that can be moved around in editor mode
        ShaderBufferSet playerBufferSet = new ShaderBufferSet();

        public EditorController(Camera camera, TextureManager textureManager) : base(camera)
        {
            this.speed = 5;
            cursor = new Cursor(new Vector3(0, 0, 0), Voxel.Type.none, textureManager); // The transparent voxel that can be moved around in editor mode
        }

        public Cursor GetCursor() { return cursor; }

        public void Activate(PlayerController playerController, TextureManager textureManager)
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
            texture.Add(textureManager.GetBindlessTextureHandle(Voxel.Type.none));

            playerBufferSet.SetPositions(position);
            playerBufferSet.SetScales(scale);
            playerBufferSet.SetTextureHandles(texture);
        }

        /// <summary>
        /// Gets the buffer set representing the player controller
        /// </summary>
        public ShaderBufferSet GetPlayerBufferSet()
        {
            return playerBufferSet;
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
            }
            camera.Update(mouse, keyboard);
            return cursor.Update(camera, keyboard, mouse, scene);
        }
    }
}
