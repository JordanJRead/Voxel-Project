using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Voxel_Project
{
    internal class Inventory
    {
        private static float iconSize = 1.0f / 20;
        private static float margin = 1.0f / 40;
        private static float stride = iconSize + margin;
        private static float height = 1.0f / 40;

        private Item selectedItem = Item.hoe;

        enum Item
        {
            hoe,
            shovel,

            max
        }

        Texture2D[] icons = new Texture2D[(int)Item.max]
        {
            new Texture2D("Images/Inventory/hoe.png"),
            new Texture2D("Images/Inventory/shovel.png"),
        };

        public void InputUpdate(MouseState mouse, KeyboardState keyboard)
        {
            for (int i = 1; i <= 9; ++i)
            {
                if (keyboard.IsKeyPressed(Keys.D0 + i))
                {
                    if (i - 1 < (int)Item.max)
                    {
                        selectedItem = (Item)(i - 1);
                    }
                }
            }
        }

        public void Draw(UIShader uiShader, float aspectRatio)
        {
            float startingPos;
            if (icons.Length % 2 == 1)
            {
                startingPos = 0.5f - icons.Length / 2 * stride;
            }
            else
            {
                startingPos = 0.5f - icons.Length / 2 * stride + stride / 2;
            }

            for (int i = 0; i < icons.Length; i++)
            {
                if (i == (int)selectedItem)
                    uiShader.Draw(icons[i], new Vector2(startingPos + i * stride, height + iconSize / 2), iconSize * 1.3f, aspectRatio);
                else
                    uiShader.Draw(icons[i], new Vector2(startingPos + i * stride, height + iconSize / 2), iconSize, aspectRatio);
            }
        }
    }
}
