using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;

namespace Voxel_Project
{
    internal class TransparentVoxelShader : ShaderBase
    {
        public TransparentVoxelShader(string vertPath, string fragPath) : base(vertPath, fragPath)
        {

        }

        public void Render(Camera camera, VertexArray vertexArray, Voxel editorVoxel, TextureManager textureManager)
        {
            GL.Disable(EnableCap.DepthTest);
            this.Use();
            SetMat4("view", camera.GetViewMatrix());
            SetMat4("projection", camera.GetProjectionMatrix());
            SetVec3("position", editorVoxel.GetPosition());
            SetULongTextureHandle("textureHandle", (ulong)textureManager.GetBindlessTextureHandle(editorVoxel.type));

            vertexArray.Use();

            const int triangleCount = 12;
            const int verticesPerTriangle = 3;
            GL.DrawArrays(PrimitiveType.Triangles, 0, triangleCount * verticesPerTriangle);
            GL.Enable(EnableCap.DepthTest);
        }
    }
}
