using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Voxel_Project
{
    /// <summary>
    /// Class for basic creation and interface with a GPU shader.
    /// </summary>
    internal class ShaderBase
    {
        int id;

        public void Use()
        {
            GL.UseProgram(id);
        }

        protected void SetInt(string name, int value)
        {
            GL.Uniform1(GL.GetUniformLocation(id, name), value);
        }

        protected void SetFloat(string name, float value)
        {
            GL.Uniform1(GL.GetUniformLocation(id, name), value);
        }

        protected void SetMat4(string name, Matrix4 value)
        {
            GL.UniformMatrix4(GL.GetUniformLocation(id, name), false, ref value);
        }

        protected void SetBool(string name, bool value)
        {
            GL.Uniform1(GL.GetUniformLocation(id, name), value ? 1 : 0); // Sets as an int
        }

        protected void SetVec3(string name, Vector3 value)
        {
            GL.Uniform3(GL.GetUniformLocation(id, name), value);
        }

        protected void SetULongTextureHandle(string name, ulong value)
        {
            GL.Arb.UniformHandle(GL.GetUniformLocation(id, name), value);
        }

        public ShaderBase(string vertPath, string fragPath)
        {
            string projectPath = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName;

            string vertSrc = File.ReadAllText(projectPath + '/' + vertPath);
            string fragSrc = File.ReadAllText(projectPath + '/' + fragPath);

            int vertShader = GL.CreateShader(ShaderType.VertexShader);
            int fragShader = GL.CreateShader(ShaderType.FragmentShader);

            GL.ShaderSource(vertShader, vertSrc);
            GL.ShaderSource(fragShader, fragSrc);

            GL.CompileShader(vertShader);
            GL.CompileShader(fragShader);

            GL.GetShader(vertShader, ShaderParameter.CompileStatus, out int successVert);
            if (successVert == 0)
            {
                string infoLog = GL.GetShaderInfoLog(vertShader);
                Console.WriteLine(infoLog);
            }

            GL.GetShader(fragShader, ShaderParameter.CompileStatus, out int successFrag);
            if (successFrag == 0)
            {
                string infoLog = GL.GetShaderInfoLog(vertShader);
                Console.WriteLine(infoLog);
            }

            id = GL.CreateProgram();

            GL.AttachShader(id, vertShader);
            GL.AttachShader(id, fragShader);

            GL.LinkProgram(id);

            GL.GetProgram(id, GetProgramParameterName.LinkStatus, out int successShader);
            if (successShader == 0)
            {
                string infoLog = GL.GetProgramInfoLog(id);
                Console.WriteLine(infoLog);
            }

            GL.DetachShader(id, vertShader);
            GL.DetachShader(id, fragShader);
            GL.DeleteShader(vertShader);
            GL.DeleteShader(fragShader);
        }
    }
}
