using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using static System.Formats.Asn1.AsnWriter;

namespace Voxel_Project
{
    /// <summary>
    /// Deals with selecting which seed to plant
    /// </summary>
    internal class SeedManager
    {
        int[] seedCounts = new int[(int)Plant.Type.none]; // Does not include Plant.Type.none
        Plant.Type selectedSeed = (Plant.Type) 0;

        Texture2D[] seedIcons = new Texture2D[(int)Plant.Type.none];

        int[] seedCosts = new int[(int)Plant.Type.none]
        {
            5,
            10,
            15
        };

        public SeedManager(int[] seedCounts)
        {
            if (seedCounts.Length != this.seedCounts.Length)
            {
                throw new ArgumentException("INCORREST NUMBER OF SEEDS");
            }

            this.seedCounts = seedCounts;
            for (int i = 0; i < seedIcons.Length; i++)
            {
                seedIcons[i] = new Texture2D($"Images/Seeds/{(Plant.Type)i}.png");
            }
        }

        public void InputUpdate(KeyboardState keyboard, MouseState mouse, Scene scene, Camera camera, Voxel? lookingAtVoxel, ResourceManager moneyManager)
        {
            if (keyboard.IsKeyPressed(Keys.Q))
            {
                CycleBackwards();
            }
            else if (keyboard.IsKeyPressed(Keys.E))
            {
                CycleForwards();
            }
            else if (keyboard.IsKeyPressed(Keys.R))
            {
                BuySeed(moneyManager);
            }

            if (mouse.IsButtonPressed(MouseButton.Left))
            {
                bool isSeedPositionVacant = false;
                if (lookingAtVoxel != null)
                {
                    // If clicking on tilled ground
                    if (lookingAtVoxel.GetVoxelType() == Voxel.Type.tilled && scene.IsPositionEmpty(lookingAtVoxel.GetPosition() + Vector3.UnitY))
                    {
                        isSeedPositionVacant = true;
                    }
                }

                if (lookingAtVoxel != null && isSeedPositionVacant && PlaceSeed())
                {
                    scene.PlantSeed(lookingAtVoxel.GetPosition() + Vector3.UnitY, GetCurrentSeedType());
                }
            }
        }

        public int[] GetSeedCounts()
        {
            return seedCounts;
        }

        /// <summary>
        /// Buys the selected seed if player has enough money
        /// </summary>
        /// <returns>The money the player has after the purchase</returns>
        public void BuySeed(ResourceManager moneyManager)
        {
            if (moneyManager.GetResourceCount() >= seedCosts[(int)selectedSeed])
            {
                seedCounts[(int)selectedSeed] += 1;
                moneyManager.ChangeResource(-seedCosts[(int)selectedSeed]);
            }
        }

        /// <summary>
        /// Removes a seed from the inventory if the player has any
        /// </summary>
        /// <returns>Whether the player has a seed</returns>
        public bool PlaceSeed()
        {
            if (seedCounts[(int)selectedSeed] > 0)
            {
                seedCounts[(int)selectedSeed]--;
                return true;
            }
            return false;
        }

        public Plant.Type GetCurrentSeedType()
        {
            return selectedSeed;
        }

        public void CycleForwards()
        {
            selectedSeed = (Plant.Type)((int)selectedSeed + 1);
            if (selectedSeed == Plant.Type.none)
            {
                selectedSeed = (Plant.Type)0;
            }
        }

        public void CycleBackwards()
        {
            selectedSeed = (Plant.Type)((int)selectedSeed - 1);
            if ((int)selectedSeed < 0)
            {
                selectedSeed = (Plant.Type)((int)Plant.Type.none - 1);
            }
        }

        public Texture2D GetCurrentIcon()
        {
            return seedIcons[(int)selectedSeed];
        }

        public int GetCurrentSeedCount()
        {
            return seedCounts[(int)selectedSeed];
        }
    }
}
