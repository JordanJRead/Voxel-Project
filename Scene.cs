using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Voxel_Project;

namespace Voxel_Project
{
    internal class Scene
    {
        List<Voxel> voxels = new List<Voxel>();
        Voxel? cursorVoxel = null; // The transparent voxel that can be moved around in editor mode

        VertexArray vertexArray;
        VertexBuffer vertexBuffer;

        InstancedVoxelShader instancedVoxelShader = new InstancedVoxelShader("Shaders/instancedvoxel.vert", "Shaders/instancedvoxel.frag");
        TransparentVoxelShader transparentVoxelShader = new TransparentVoxelShader("Shaders/transparentvoxel.vert", "Shaders/transparentvoxel.frag");
        TextureManager textureManager = new TextureManager();
        string initialPath;

        /// <summary>
        /// Loads scene data from a file path into program memory
        /// </summary>
        public Scene(string filePath)
        {
            cursorVoxel = new Voxel(new Vector3(0, 3, 0), Voxel.Type.grass);

            string projectPath = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName;
            this.initialPath = projectPath + '/' + filePath;

            string[] fileLines = File.ReadLines(this.initialPath).ToArray();

            foreach (string line in fileLines)
            {
                if (line == "")
                    continue;

                string[] voxelInfo = line.Split(',');
                Vector3 pos = new Vector3();
                pos.X = float.Parse(voxelInfo[0]);
                pos.Y = float.Parse(voxelInfo[1]);
                pos.Z = float.Parse(voxelInfo[2]);
                voxels.Add(new Voxel(pos, voxelInfo[3]));
            }

            instancedVoxelShader.UpdateGPUVoxelData(voxels, textureManager);

            // Cube vertices
            // x1, y1, z1, x2, y2, z2, etc.
            // Cube is centerd on (0, 0, 0) and has dimensions of 1 (-0.5 to 0.5)
            /*
              Y
              |
              |
              |
              |
              ----------X
             /
            /
           Z
            */
            float right = 0.5f;
            float left = -0.5f;
            float up = 0.5f;
            float down = -0.5f;
            float near = 0.5f;
            float far = -0.5f;

            float[] vertices =
            {
                // Front face
                left,  up,   near, 0, 0, near,
                left,  down, near, 0, 0, near,
                right, down, near, 0, 0, near,

                right, down, near, 0, 0, near,
                right, up,   near, 0, 0, near,
                left,  up,   near, 0, 0, near,

                // Back face
                right, down, far, 0, 0, far,
                left,  down, far, 0, 0, far,
                left,  up,   far, 0, 0, far,

                left,  up,   far, 0, 0, far,
                right, up,   far, 0, 0, far,
                right, down, far, 0, 0, far,

                // Right face
                right, up,   far, right, 0, 0,
                right, up,   near, right, 0, 0,
                right, down, near, right, 0, 0,

                right, down, near, right, 0, 0,
                right, down, far, right, 0, 0,
                right, up,   far, right, 0, 0,
                
                // Left face
                left, down, near, left, 0, 0,
                left, up,   near, left, 0, 0,
                left, up,   far, left, 0, 0,

                left, up,   far, left, 0, 0,
                left, down, far, left, 0, 0,
                left, down, near, left, 0, 0,

                // Top face
                left,  up, far, 0, up, 0,
                left,  up, near, 0, up, 0,
                right, up, near, 0, up, 0,

                right, up, near, 0, up, 0,
                right, up, far, 0, up, 0,
                left,  up, far, 0, up, 0,

                // Bottom face
                right, down, near, 0, down, 0,
                left,  down, near, 0, down, 0,
                left,  down, far, 0, down, 0,

                left,  down, far, 0, down, 0,
                right, down, far, 0, down, 0,
                right, down, near, 0, down, 0,
            };
            vertexBuffer = new VertexBuffer(vertices);
            vertexArray = new VertexArray([3, 3], vertexBuffer);
        }

        public void Print()
        {
            foreach (Voxel voxel in voxels)
            {
                Console.WriteLine(voxel.ToString());
            }
        }

        /// <summary>
        /// Writes scene data to the same file path that the scene was loaded from, overwriting that file
        /// </summary>
        public void Save()
        {
            Save(initialPath);
        }

        /// <summary>
        /// Writes scene data to a file. If the file exists, it gets overwriten. If the file does not exist, it get created
        /// </summary>
        /// <param name="filePath"></param>
        public void Save(string filePath)
        {
            voxels[0].position = new Vector3(11, 12, 13);
            voxels[0].type = Voxel.Type.none;
            File.Delete(filePath);
            string fileSrc = "";
            foreach (Voxel voxel in voxels)
            {
                fileSrc += voxel.position.X;
                fileSrc += ',';
                fileSrc += voxel.position.Y;
                fileSrc += ',';
                fileSrc += voxel.position.Z;
                fileSrc += ',';
                fileSrc += voxel.GetTypeName();
                fileSrc += '\n';
            }

            using (StreamWriter sw = new StreamWriter(filePath))
            {
                sw.Write(fileSrc);
            }
        }

        public void Render(Camera camera)
        {
            instancedVoxelShader.Render(camera, vertexArray);
            if (cursorVoxel != null)
            {
                transparentVoxelShader.Render(camera, vertexArray, cursorVoxel, textureManager);
            }
        }

        /// <summary>
        /// Mostly deals with moving the editor's 'selection' cube
        /// </summary>
        public void Update(KeyboardState keyboard, MouseState mouse, Camera camera)
        {
            if (cursorVoxel == null)
                return;

            if (keyboard.IsKeyDown(Keys.LeftControl))
            {
                Vector3 cameraForward = camera.GetForward();
                Vector3 cursorForwardAxis = new Vector3(1, 0, 0);

                int[] axisValues = { -1, 1 };

                Vector3 currentAxis;

                // Find the best axis to move the cube along
                // The dot product returns a greater value the more aligned the vectors are
                currentAxis = Vector3.UnitX;
                if (Vector3.Dot(cameraForward, currentAxis) > Vector3.Dot(cameraForward, cursorForwardAxis))
                {
                    cursorForwardAxis = currentAxis;
                }
                currentAxis = -Vector3.UnitX;
                if (Vector3.Dot(cameraForward, currentAxis) > Vector3.Dot(cameraForward, cursorForwardAxis))
                {
                    cursorForwardAxis = currentAxis;
                }
                currentAxis = Vector3.UnitZ;
                if (Vector3.Dot(cameraForward, currentAxis) > Vector3.Dot(cameraForward, cursorForwardAxis))
                {
                    cursorForwardAxis = currentAxis;
                }
                currentAxis = -Vector3.UnitZ;
                if (Vector3.Dot(cameraForward, currentAxis) > Vector3.Dot(cameraForward, cursorForwardAxis))
                {
                    cursorForwardAxis = currentAxis;
                }

                Vector3 rightAxis = Vector3.Cross(cursorForwardAxis, Vector3.UnitY);

                // Move cursor
                if (keyboard.IsKeyPressed(Keys.W))
                {
                    cursorVoxel.position += cursorForwardAxis;
                }
                if (keyboard.IsKeyPressed(Keys.S))
                {
                    cursorVoxel.position -= cursorForwardAxis;
                }
                if (keyboard.IsKeyPressed(Keys.A))
                {
                    cursorVoxel.position -= rightAxis;
                }
                if (keyboard.IsKeyPressed(Keys.D))
                {
                    cursorVoxel.position += rightAxis;
                }
                if (keyboard.IsKeyPressed(Keys.Space))
                {
                    cursorVoxel.position += Vector3.UnitY;
                }
                if (keyboard.IsKeyPressed(Keys.LeftShift))
                {
                    cursorVoxel.position -= Vector3.UnitY;
                }

                // Cycle through cursor voxel types
                // Voxel.Type.none is the maximum enum value
                if (keyboard.IsKeyPressed(Keys.Q)) // Reverse
                {
                    int newType = (int)cursorVoxel.type - 1;
                    if (newType < 0)
                    {
                        newType = (int)Voxel.Type.none;
                    }
                    cursorVoxel.type = (Voxel.Type)(newType);
                }
                if (keyboard.IsKeyPressed(Keys.E)) // Forward
                {
                    int newType = (int)cursorVoxel.type + 1;
                    if (newType > (int)Voxel.Type.none)
                    {
                        newType = 0;
                    }
                    cursorVoxel.type = (Voxel.Type)(newType);
                }

                // Modifying voxels
                Voxel? selectedVoxel = GetSelectedVoxel();
                bool hasSceneChanged = false;
                if (selectedVoxel != null)
                {
                    // Replacing voxel
                    if (mouse.IsButtonPressed(MouseButton.Left))
                    {
                        selectedVoxel.type = cursorVoxel.type;
                        hasSceneChanged = true;
                    }
                    // Deleting voxel
                    if (mouse.IsButtonPressed(MouseButton.Right))
                    {
                        voxels.Remove(selectedVoxel);
                        hasSceneChanged = true;
                    }
                }
                else
                {
                    // Placing voxel
                    if (mouse.IsButtonPressed(MouseButton.Left))
                    {
                        voxels.Add(new Voxel(cursorVoxel.position, cursorVoxel.type));
                        hasSceneChanged = true;
                    }
                }

                // Update GPU
                if (hasSceneChanged)
                {
                    instancedVoxelShader.UpdateGPUVoxelData(voxels, textureManager);
                }
            }
        }

        /// <summary>
        /// Checks which voxel the editor's cursor is overlapping, if any
        /// </summary>
        /// <returns></returns>
        private Voxel? GetSelectedVoxel()
        {
            if (cursorVoxel == null)
                return null;

            foreach (Voxel voxel in voxels)
            {
                // If positions are equal
                if ((voxel.position - cursorVoxel.position).Length < 0.01)
                {
                    return voxel;
                }
            }
            return null;
        }
    }
}
