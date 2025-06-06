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
        UIShader uiShader;
        int screenWidth;
        int screenHeight;
        Texture2D testTexture;

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
            testTexture = new Texture2D("");
            uiShader = new UIShader("Shaders/ui.vert", "shaders/ui.frag");
            screenWidth = width;
            screenHeight = height;
            ExtensionsCheck();
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            scene = new Scene("scene.txt", screenWidth, screenHeight);

            Vector3 playerPosition = scene.GetInitialPlayerPosition();
            int money = scene.GetInitialPlayerMoney();
            int wood = scene.GetInitialPlayerWood();
            int[] seedCounts = scene.GetInitialPlayerSeedCounts();

            playerController = new PlayerController(playerPosition, new Camera(width, height), money, wood, seedCounts);
            editorController = new EditorController(new Camera(width, height), scene.GetTextureManager());
            editorController.Activate(playerController, scene.GetTextureManager());
            currentController = playerController;
            //currentCamera = editorCamera;
            CursorState = CursorState.Grabbed;
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            if (KeyboardState.IsKeyPressed(Keys.P))
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
                scene.UpdateGPU(KeyboardState, MouseState);
            }
            scene.FrameUpdate((float)e.Time);
            //Console.WriteLine($"FPS: {1.0f / e.Time}");
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            if (currentController == editorController)
            {
                scene.Render(currentController.GetCamera(), editorController.GetCursor());
                scene.RenderCubeBufferSet(currentController.GetCamera(), editorController.GetPlayerCubeBufferSet());
            }
            else
            {
                scene.Render(currentController.GetCamera());
                playerController.DrawUI(uiShader, ((float)screenWidth) / screenHeight);
            }

            SwapBuffers();
        }
        protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
        {
            GL.Viewport(0, 0, e.Width, e.Height);
            playerController.GetCamera().Resize(e.Width, e.Height);
            editorController.GetCamera().Resize(e.Width, e.Height);
            scene.Resize(e.Width, e.Height);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            scene.Save(playerController);
        }
    }
}
