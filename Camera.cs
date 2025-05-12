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

        int screenWidth;
        int screenHeight;

        Vector3 position = new Vector3();
        Vector2 prevMousePos = new Vector2();

        public Camera(int screenWidth, int screenHeight, Vector3 position)
        {
            this.position = position;
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;
        }

        public void Update(MouseState mouse)
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

        public Matrix4 GetViewMatrix()
        {
            Vector3 forward = new Vector3();

            // Taken from https://opentk.net/learn/chapter1/9-camera.html
            forward.X = (float)Math.Cos(MathHelper.DegreesToRadians(pitch)) * (float)Math.Cos(MathHelper.DegreesToRadians(yaw));
            forward.Y = (float)Math.Sin(MathHelper.DegreesToRadians(pitch));
            forward.Z = (float)Math.Cos(MathHelper.DegreesToRadians(pitch)) * (float)Math.Sin(MathHelper.DegreesToRadians(yaw));
            forward = Vector3.Normalize(forward);

            return Matrix4.LookAt(position, position + forward, Vector3.UnitY);
        }

        public Matrix4 GetProjectionMatrix()
        {
            return Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(80), screenWidth / screenHeight, 0.1f, 100);
        }
    }
}
