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
using System.ComponentModel;

namespace Voxel_Project
{
    internal class App : GameWindow
    {
        Scene scene;
        EditorCamera editorCamera;
        PlayerCamera playerCamera;
        CameraBase currentCamera;

        unsafe static void ExtensionsCheck()
        {
            int extensionsCount;
            int* ptr = &extensionsCount;
            GL.GetInteger(GetPName.NumExtensions, ptr);
            bool found = false;
            for (int i = 0; i < extensionsCount; ++i)
            {
                string extensionName = GL.GetString(StringNameIndexed.Extensions, i);
                if (extensionName == "GL_ARB_bindless_texture")
                {
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                Console.WriteLine("ERROR: Missing OpenGL extension GL_ARB_bindless_texture");
            }
        }

        unsafe public App(int width, int height, string title)
            : base(new GameWindowSettings() { UpdateFrequency = 0 }, new NativeWindowSettings() { Size = (width, height), Title = title, APIVersion = new System.Version(4, 3) })
        {
            ExtensionsCheck();
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            scene = new Scene("scene.txt");
            editorCamera = new EditorCamera(width, height, new Vector3(0, 0, 0));
            playerCamera = new PlayerCamera(width, height, new Vector3(0, 3, 0));
            currentCamera = playerCamera;
            //currentCamera = editorCamera;
            CursorState = CursorState.Grabbed;
            GL.ClearColor(0.2f, 0.2f, 0.2f, 1);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            if (KeyboardState.IsKeyDown(Keys.Escape))
            {
                Close();
            }
            currentCamera.Update(MouseState, KeyboardState, (float)e.Time, scene);
            scene.Update(KeyboardState, MouseState, currentCamera);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            scene.Render(currentCamera);

            SwapBuffers();
        }
        protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
        {
            GL.Viewport(0, 0, e.Width, e.Height);
            currentCamera.Resize(e.Width, e.Height);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            scene.Save();
        }
    }
}
