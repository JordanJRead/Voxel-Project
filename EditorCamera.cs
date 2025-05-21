using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Voxel_Project
{
    internal class EditorCamera : CameraBase
    {
        public EditorCamera(int screenWidth, int screenHeight, Vector3 position, float speed = 5) : base(screenWidth, screenHeight, position, speed)
        {
        }

        public override void Update(MouseState mouse, KeyboardState keyboard, float deltaTime, Scene s)
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

                Vector3 forward = GetForward();
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

                MoveBy(moveVector * deltaTime * speed);
            }
        }
    }
}
