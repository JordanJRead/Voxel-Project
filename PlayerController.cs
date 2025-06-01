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
    /// Represents a player that can play the game
    /// </summary>
    internal class PlayerController : ControllerBase
    {
        Vector3 position;
        Vector3 scale = new Vector3(1, 2, 1);
        Vector3 cameraOffset = new Vector3(0, 0.5f, 0);
        Inventory inventory;
        Texture2D crosshair = new Texture2D("Images/inventory/crosshair.png");
        MoneyManager moneyManager = new MoneyManager();

        public PlayerController(Vector3 position, Camera camera) : base(camera)
        {
            this.speed = 2;
            this.position = position;
            this.camera.SetPosition(position);
            inventory = new Inventory();
        }

        public override bool Update(MouseState mouse, KeyboardState keyboard, float deltaTime, Scene scene)
        {
            if (!keyboard.IsKeyDown(Keys.LeftControl))
            {
                Vector3 moveVector = new Vector3();

                Vector3 badForward = camera.GetForward();
                Vector3 forward = new Vector3(badForward.X, 0, badForward.Z).Normalized();
                Vector3 right = camera.GetRight();

                //if (keyboard.IsKeyDown(Keys.Space))
                //    moveVector += Vector3.UnitY;
                //if (keyboard.IsKeyDown(Keys.LeftShift))
                //    moveVector -= Vector3.UnitY;
                if (keyboard.IsKeyDown(Keys.W))
                    moveVector += forward;
                if (keyboard.IsKeyDown(Keys.S))
                    moveVector -= forward;
                if (keyboard.IsKeyDown(Keys.A))
                    moveVector -= right;
                if (keyboard.IsKeyDown(Keys.D))
                    moveVector += right;
                if (moveVector.X != 0 || moveVector.Y != 0 || moveVector.Z != 0)
                    moveVector.Normalize();
                position = PhysicsManager.MoveInScene(this, scene, moveVector * deltaTime * speed);
                camera.SetPosition(position + cameraOffset);
            }

            camera.Update(mouse, keyboard);
            moneyManager.Update(deltaTime);
            return inventory.InputUpdate(mouse, keyboard, scene, camera, 10, moneyManager);
        }

        public void DrawUI(UIShader uiShader, float aspectRatio)
        {
            inventory.Draw(uiShader, aspectRatio);
            uiShader.Draw(crosshair, new Vector2(0.5f, 0.5f), 0.05f, aspectRatio);
            moneyManager.Draw(uiShader, aspectRatio);
        }

        public Vector3 GetPosition()
        {
            return position;
        }

        public Vector3 GetSize()
        {
            return scale;
        }

        public void MoveBy(Vector3 move)
        {
            position += move;
        }
    }
}
