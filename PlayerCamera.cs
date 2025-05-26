using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Voxel_Project
{
    internal class PlayerCamera : CameraBase
    {
        PhysicsManager physicsManager = new PhysicsManager();
        Vector3 playerCenterOffset = new Vector3(0, -0.5f, 0);
        Vector3 playerPhysicsScale = new Vector3(1, 2, 1);
        public PlayerCamera(int screenWidth, int screenHeight, Vector3 position, float speed = 5, float yaw = 0) : base(screenWidth, screenHeight, position, speed, yaw)
        {
        }

        public override void Update(MouseState mouse, KeyboardState keyboard, float deltaTime, Scene scene)
        {
            if (isFirstMove)
            {
                prevMousePos = mouse.Position;
                isFirstMove = false;
            }

            Vector2 mouseMovement = mouse.Position - prevMousePos;
            prevMousePos = mouse.Position;

            yaw -= mouseMovement.X * sensitivity;
            pitch -= mouseMovement.Y * sensitivity;

            if (yaw < 0)
                yaw += 360;
            if (pitch < 0)
                pitch += 360;
            yaw %= 360;
            pitch %= 360;

            // Stop from going too high up
            if (pitch > 89 && pitch < 180)
                pitch = 89;

            // Stop from going straight down
            if (pitch < 270 && pitch >= 180)
                pitch = 271;

            if (!keyboard.IsKeyDown(Keys.LeftControl))
            {
                Vector3 moveVector = new Vector3();

                Vector3 badForward = GetForward();
                Vector3 forward = new Vector3(badForward.X, 0, badForward.Z).Normalized();
                Vector3 right = GetRight();

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
                if (moveVector.X != 0 || moveVector.Y != 0 || moveVector.Z != 0)
                    moveVector.Normalize();
                position = physicsManager.MoveInScene(this, scene, moveVector * deltaTime * speed);
            }
        }

        public Vector3 GetCenterOffset()
        {
            return playerCenterOffset;
        }

        public Vector3 GetPhysicsScale()
        {
            return playerPhysicsScale;
        }
    }
}
