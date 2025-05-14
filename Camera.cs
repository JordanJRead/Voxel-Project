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
    internal class Camera
    {
        // yaw and pitch are in degrees
        float pitch = 0;
        float yaw = 0;
        float sensitivity = 0.1f;
        bool isFirstMove = true; // Stops the camera from jerking quickly when loaded
        float speed = 5;

        int screenWidth;
        int screenHeight;

        Vector3 position = new Vector3();
        Vector2 prevMousePos = new Vector2();

        public Camera(int screenWidth, int screenHeight, Vector3 position, float speed = 5)
        {
            this.position = position;
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;
            this.speed = speed;
        }

        public void Update(MouseState mouse, KeyboardState keyboard, float deltaTime)
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

        public Matrix4 GetViewMatrix()
        {
            Vector3 forward = GetForward();
            return Matrix4.LookAt(position, position + forward, Vector3.UnitY);
        }

        public Matrix4 GetProjectionMatrix()
        {
            return Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(80), (float)screenWidth / screenHeight, 0.1f, 100);
        }

        public void MoveBy(Vector3 moveBy)
        {
            position += moveBy;
        }

        public Vector3 GetForward()
        {
            Vector3 forward = new Vector3();

            // Taken from https://opentk.net/learn/chapter1/9-camera.html
            forward.X = (float)Math.Cos(MathHelper.DegreesToRadians(pitch)) * (float)Math.Cos(MathHelper.DegreesToRadians(yaw));
            forward.Y = (float)Math.Sin(MathHelper.DegreesToRadians(pitch));
            forward.Z = (float)Math.Cos(MathHelper.DegreesToRadians(pitch)) * -(float)Math.Sin(MathHelper.DegreesToRadians(yaw));
            forward = Vector3.Normalize(forward);
            return forward;
        }

        public Vector3 GetRight()
        {
            Vector3 right = new Vector3();
            right.X = (float)Math.Sin(MathHelper.DegreesToRadians(yaw));
            right.Y = 0;
            right.Z = (float)Math.Cos(MathHelper.DegreesToRadians(yaw));
            right = Vector3.Normalize(right);
            return right;
        }

        public void Resize(int width, int height)
        {
            this.screenWidth = width;
            this.screenHeight = height;
        }
    }
}
