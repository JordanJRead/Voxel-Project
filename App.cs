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
        EditorController editorController;
        PlayerController playerController;
        ControllerBase currentController;

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
                Console.WriteLine("ERROR: Missing OpenGL extension GL_ARB_bindless_texture. Get a better GPU");
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
            playerController = new PlayerController(new Vector3(0, 0, 0), new Camera(width, height));
            editorController = new EditorController(new Camera(width, height), scene.GetTextureManager());
            editorController.Activate(playerController, scene.GetTextureManager());
            currentController = editorController;
            //currentCamera = editorCamera;
            CursorState = CursorState.Grabbed;
            GL.ClearColor(0.2f, 0.2f, 0.2f, 1);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            if (KeyboardState.IsKeyPressed(Keys.C))
            {
                if (currentController == editorController)
                    currentController = playerController;
                else
                {
                    currentController = editorController;
                    editorController.Activate(playerController, scene.GetTextureManager());
                }
            }
            if (KeyboardState.IsKeyDown(Keys.Escape))
            {
                Close();
            }
            if (currentController.Update(MouseState, KeyboardState, (float)e.Time, scene))
            {
                scene.Update(KeyboardState, MouseState);
            }
            Console.WriteLine($"FPS: {1.0f / e.Time}");
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            if (currentController == editorController)
            {
                scene.Render(currentController.GetCamera(), editorController.GetCursor());
                scene.RenderBufferSet(currentController.GetCamera(), editorController.GetPlayerBufferSet());
            }
            else
            {
                scene.Render(currentController.GetCamera());
            }
            SwapBuffers();
        }
        protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
        {
            GL.Viewport(0, 0, e.Width, e.Height);
            playerController.GetCamera().Resize(e.Width, e.Height);
            editorController.GetCamera().Resize(e.Width, e.Height);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            scene.Save();
        }
    }
}
