using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Voxel_Project;
using OpenTK.Mathematics;

namespace Voxel_Project
{
    internal class App : GameWindow
    {
        int shader;
        Scene scene;
        Camera camera;
        VertexBuffer vertexBuffer;
        VertexArray vertexArray;

        unsafe public App(int width, int height, string title)
            : base(GameWindowSettings.Default, new NativeWindowSettings() { Size = (width, height), Title = title, APIVersion = new System.Version(4, 3) })
        {
            scene = new Scene("scene.txt");
            camera = new Camera(width, height, new Vector3(0, 0, 0));
            CursorState = CursorState.Grabbed;
            GL.ClearColor(0.2f, 0.2f, 0.2f, 1);

            float[] vertices = new float[]
            {
                -0.5f, -0.5f, 0, 1, 0, 0,
                 0.5f, -0.5f, 0, 0, 1, 0,
                 0.0f,  0.5f, 0, 0, 0, 1
            };

            vertexBuffer = new VertexBuffer(vertices);
            vertexArray = new VertexArray([3, 3], vertexBuffer);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            if (KeyboardState.IsKeyDown(Keys.Escape))
            {
                Close();
            }
            camera.Update(MouseState, KeyboardState, (float)e.Time);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);// | ClearBufferMask.DepthBufferBit);

            scene.Render(camera);

            //vertexArray.Use();
            //ShaderBase helloShader = new ShaderBase("Shaders/hello.vert", "Shaders/hello.frag");
            //helloShader.Use();
            //GL.DrawArrays(PrimitiveType.Triangles, 0, 3);

            SwapBuffers();
        }
        protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
        {
            GL.Viewport(0, 0, e.Width, e.Height);
        }
    }
}
