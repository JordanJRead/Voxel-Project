using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Voxel_Project
{
    internal class Inventory
    {
        private static float slotWidth = 1.0f / 20;
        private static float margin = 1.0f / 40;
        private static float stride = slotWidth + margin;
        private static float height = 1.0f / 40;
        private static int fenceValue = 5;

        private static float seedIconWidth = slotWidth * 0.5f;
        private static float seedIconCenterOffset = -(slotWidth - seedIconWidth) * 0.5f;

        private Item selectedItem = Item.hoe;
        private SeedManager seedManager = new SeedManager();

        enum Item
        {
            hoe,
            seedManager, // Planting seeds
            scythe,
            axe,
            hammer
        }

        Texture2D[] itemIcons = new Texture2D[Enum.GetNames<Item>().Length];

        Texture2D blackBorder = new Texture2D("Images/Inventory/blackBorder.png");
        Texture2D redBorder = new Texture2D("Images/Inventory/redBorder.png");
        Texture2D yellowBorder = new Texture2D("Images/Inventory/yellowBorder.png");
        Texture2D greenBorder = new Texture2D("Images/Inventory/greenBorder.png");

        Texture2D background = new Texture2D("Images/Inventory/background.png");

        public Inventory()
        {
            for (int i = 0; i < itemIcons.Length; ++i)
            {
                itemIcons[i] = new Texture2D($"Images/Inventory/{(Item)i}.png");
            }
        }

        /// <summary>
        /// Per-frame update for the inventory
        /// </summary>
        /// <returns>Whether the scene has changed or not</returns>
        public bool InputUpdate(MouseState mouse, KeyboardState keyboard, Scene scene, Camera camera, ResourceManager moneyManager, ResourceManager woodManager)
        {
            bool hasSceneChanged = false;
            Voxel? lookingAtVoxel = PhysicsManager.RayTraceVoxel(camera.GetPosition(), camera.GetForward(), 5, scene);

            // Select item
            for (int num = 1; num <= 9; ++num)
            {
                if (keyboard.IsKeyPressed(Keys.D0 + num))
                {
                    if (num <= Enum.GetNames<Item>().Length)
                    {
                        selectedItem = (Item)(num - 1);
                    }
                }
            }

            switch (selectedItem)
            {
                case Item.hoe:
                    if (mouse.IsButtonPressed(MouseButton.Left) && lookingAtVoxel != null)
                    {
                        if (lookingAtVoxel.GetVoxelType() == Voxel.Type.grass)
                        {
                            lookingAtVoxel.SetType(Voxel.Type.tilled);
                            hasSceneChanged = true;
                        }
                        else if (lookingAtVoxel.GetVoxelType() == Voxel.Type.tilled && scene.GetPlantOnVoxel(lookingAtVoxel) == null)
                        {
                            lookingAtVoxel.SetType(Voxel.Type.grass);
                            hasSceneChanged = true;
                        }
                    }
                    break;

                case Item.seedManager:
                    seedManager.InputUpdate(keyboard, mouse, scene, camera, lookingAtVoxel, moneyManager);
                    break;

                case Item.scythe:
                    if (mouse.IsButtonPressed(MouseButton.Left) && lookingAtVoxel != null)
                    {
                        Plant? plant = scene.GetPlantOnVoxel(lookingAtVoxel);
                        if (plant != null)
                        {
                            if (plant.GetGrowth() >= 1.0f)
                            {
                                moneyManager.ChangeResource(scene.HarvestPlant(plant));
                                scene.HarvestPlant(plant);
                                hasSceneChanged = true;
                            }
                        }
                    }
                    break;

                case Item.axe:
                    if (lookingAtVoxel != null && mouse.IsButtonPressed(MouseButton.Left))
                    {
                        if (lookingAtVoxel.GetVoxelType() == Voxel.Type.log)
                        {
                            hasSceneChanged = true;
                            scene.RemoveTree(lookingAtVoxel.GetPosition());
                            woodManager.ChangeResource(10);
                        }
                    }
                    break;

                case Item.hammer:
                    // Place fence
                    if (woodManager.GetResourceCount() >= fenceValue)
                    {
                        if (lookingAtVoxel != null && mouse.IsButtonPressed(MouseButton.Left))
                        {
                            if (scene.IsPositionEmpty(lookingAtVoxel.GetPosition() + Vector3.UnitY))
                            {
                                scene.AddFence(new Fence(new Vector3(lookingAtVoxel.GetPosition() + Vector3.UnitY)));
                                woodManager.ChangeResource(-fenceValue);
                                hasSceneChanged = true;
                            }
                        }
                    }

                    // Break fence
                    if (lookingAtVoxel != null && mouse.IsButtonPressed(MouseButton.Right))
                    {
                        Fence? fence = scene.GetFenceManager().GetFenceAtPosition(lookingAtVoxel.GetPosition() + Vector3.UnitY);
                        if (fence != null)
                        {
                            scene.GetFenceManager().RemoveFence(fence);
                            woodManager.ChangeResource(fenceValue);
                            hasSceneChanged = true;
                        }
                    }
                    break;
            }

            return hasSceneChanged;
        }

        public void Draw(UIShader uiShader, float aspectRatio)
        {
            float startingPos;
            if (itemIcons.Length % 2 == 1)
            {
                startingPos = 0.5f - itemIcons.Length / 2 * stride;
            }
            else
            {
                startingPos = 0.5f - itemIcons.Length / 2 * stride + stride / 2;
            }

            for (int i = 0; i < itemIcons.Length; i++)
            {
                Vector2 slotPosition = new Vector2(startingPos + i * stride, height + slotWidth / 2);
                float scale = i == (int)selectedItem ?  1.3f : 1;

                uiShader.Draw(background, slotPosition, slotWidth * scale, aspectRatio);
                uiShader.Draw(itemIcons[i], slotPosition, slotWidth * scale, aspectRatio);

                // Seeds
                if (i == (int)Item.seedManager)
                {
                    uiShader.Draw(seedManager.GetCurrentIcon(), slotPosition + new Vector2(seedIconCenterOffset * scale, seedIconCenterOffset * scale * aspectRatio), seedIconWidth * scale, aspectRatio);

                    Texture2D border;
                    int seedCount = seedManager.GetCurrentSeedCount();

                    if (seedCount == 0)
                    {
                        border = blackBorder;
                    }
                    else if (seedCount <= 5)
                    {
                        border = redBorder;
                    }
                    else if (seedCount <= 15)
                    {
                        border = yellowBorder;
                    }
                    else
                    {
                        border = greenBorder;
                    }
                    uiShader.Draw(border, slotPosition + new Vector2(seedIconCenterOffset * scale, seedIconCenterOffset * scale * aspectRatio), seedIconWidth * scale, aspectRatio);
                }
            }
        }
    }
}
