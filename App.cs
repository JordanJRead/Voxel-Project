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
        PlayerEditor playerEditor;
        PlayerGame playerGame;
        PlayerBase currentPlayer;

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
            playerEditor = new PlayerEditor(new Camera(width, height, new Vector3(0, 0, 0)), scene.GetTextureManager());
            playerGame = new PlayerGame(new Vector3(0, 0, 0), new Camera(width, height, new Vector3(0, 0, 0))); // TODO change
            currentPlayer = playerEditor;
            //currentCamera = editorCamera;
            CursorState = CursorState.Grabbed;
            GL.ClearColor(0.2f, 0.2f, 0.2f, 1);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            if (KeyboardState.IsKeyPressed(Keys.C))
            {
                if (currentPlayer == playerEditor)
                    currentPlayer = playerGame;
                else
                    currentPlayer = playerEditor;
            }
            if (KeyboardState.IsKeyDown(Keys.Escape))
            {
                Close();
            }
            if (currentPlayer.Update(MouseState, KeyboardState, (float)e.Time, scene))
            {
                scene.Update(KeyboardState, MouseState);
            }
            Console.WriteLine($"FPS: {1.0f / e.Time}");
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            if (currentPlayer == playerEditor)
            {
                scene.Render(currentPlayer.GetCamera(), playerEditor.GetCursor());
            }
            else
            {
                scene.Render(currentPlayer.GetCamera());
            }
            SwapBuffers();
        }
        protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
        {
            GL.Viewport(0, 0, e.Width, e.Height);
            playerGame.GetCamera().Resize(e.Width, e.Height);
            playerEditor.GetCamera().Resize(e.Width, e.Height);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            scene.Save();
        }
    }
}
