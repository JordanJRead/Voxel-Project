using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Voxel_Project
{
    internal abstract class CameraBase
    {
        // yaw and pitch are in degrees
        protected float pitch = 0;
        protected float yaw = 0;
        protected float sensitivity = 0.1f;
        protected bool isFirstMove = true; // Stops the camera from jerking quickly when loaded
        protected float speed = 5;

        protected int screenWidth;
        protected int screenHeight;

        protected Vector3 position = new Vector3();
        protected Vector2 prevMousePos = new Vector2();

        public CameraBase(int screenWidth, int screenHeight, Vector3 position, float speed = 5, float yaw = 0)
        {
            this.position = position;
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;
            this.speed = speed;
            this.yaw = yaw;
        }

        public abstract void Update(MouseState mouse, KeyboardState keyboard, float deltaTime, Scene scene);

        public Vector3 GetPosition()
        {
            return position;
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
