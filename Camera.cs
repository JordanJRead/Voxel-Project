using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Voxel_Project
{
    internal class Camera
    {
        // yaw and pitch are in degrees
        protected float pitch = 0;
        protected float yaw = 0;
        protected float sensitivity = 0.1f;
        protected bool isFirstMove = true; // Stops the camera from jerking quickly when loaded

        protected int screenWidth;
        protected int screenHeight;

        protected Vector3 position = new Vector3();
        protected Vector2 prevMousePos = new Vector2();

        public Camera(int screenWidth, int screenHeight, Vector3 position = new Vector3(), float speed = 5, float yaw = 0)
        {
            this.position = position;
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;
            this.yaw = yaw;
        }

        public Vector3 GetPosition()
        {
            return position;
        }

        public void SetPosition(Vector3 pos)
        {
            this.position = pos;
        }

        public Matrix4 GetViewMatrix()
        {
            Vector3 forward = GetForward();
            return Matrix4.LookAt(position, position + forward, Vector3.UnitY);
        }

        public Matrix4 GetProjectionMatrix()
        {
            return Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(80), (float)screenWidth / screenHeight, 0.1f, 10000);
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

        public void Update(MouseState mouse, KeyboardState keyboard)
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
        }
    }
}
