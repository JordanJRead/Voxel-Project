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

namespace OpenTK_Test
{
    internal class App : GameWindow
    {
        int shader;
        Scene scene = new Scene("scene.txt");
        Camera camera = new Camera(new Vector3(0, 0, 0));

        unsafe public App(int width, int height, string title)
            : base(GameWindowSettings.Default, new NativeWindowSettings() { Size = (width, height), Title = title })
        {
            CursorState = CursorState.Grabbed;
            GL.ClearColor(0.2f, 0.2f, 0.2f, 1);

            // SHADER
            //string vertSrc = File.ReadAllText("color.vert");
            //string fragSrc = File.ReadAllText("color.frag");

            //int vertShader = GL.CreateShader(ShaderType.VertexShader);
            //int fragShader = GL.CreateShader(ShaderType.FragmentShader);

            //GL.ShaderSource(vertShader, vertSrc);
            //GL.ShaderSource(fragShader, fragSrc);

            //GL.CompileShader(vertShader);
            //GL.CompileShader(fragShader);

            //GL.GetShader(vertShader, ShaderParameter.CompileStatus, out int successVert);
            //if (successVert == 0)
            //{
            //    string infoLog = GL.GetShaderInfoLog(vertShader);
            //    Console.WriteLine(infoLog);
            //}

            //GL.GetShader(fragShader, ShaderParameter.CompileStatus, out int successFrag);
            //if (successFrag == 0)
            //{
            //    string infoLog = GL.GetShaderInfoLog(vertShader);
            //    Console.WriteLine(infoLog);
            //}

            //shader = GL.CreateProgram();

            //GL.AttachShader(shader, vertShader);
            //GL.AttachShader(shader, fragShader);

            //GL.LinkProgram(shader);

            //GL.GetProgram(shader, GetProgramParameterName.LinkStatus, out int successShader);
            //if (successShader == 0)
            //{
            //    string infoLog = GL.GetProgramInfoLog(shader);
            //    Console.WriteLine(infoLog);
            //}

            //GL.DetachShader(shader, vertShader);
            //GL.DetachShader(shader, fragShader);
            //GL.DeleteShader(vertShader);
            //GL.DeleteShader(fragShader);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            if (KeyboardState.IsKeyDown(Keys.Escape))
            {
                Close();
            }
            camera.Update(MouseState);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);

            //GL.UseProgram(shader);
            //GL.DrawArrays(PrimitiveType.Triangles, 0, 3);

            SwapBuffers();
        }
        protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
        {
            GL.Viewport(0, 0, e.Width, e.Height);
        }
    }
}
