using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Voxel_Project
{
    internal class MoneyNotification
    {
        static float visibleDuration = 2; // seconds
        static float shrinkingDuration = 0.5f;

        static float baseSize = 0.03f; // before scaling
        static float margin = baseSize / 4;

        static bool hasLoadedTextures = false;
        static Texture2D[] numberTextures = new Texture2D[10];
        static Texture2D plusTexture = new Texture2D("Images/Money/plus.png");
        static Texture2D minusTexture = new Texture2D("Images/Money/minus.png");

        float timeAlive = 0;
        float heightScale = 1;
        int value;
        bool isTotalMoney = false; // Whether this notification represents all of the money the player has (changes rendering)

        public MoneyNotification(int value, bool isTotalMoney = false)
        {
            this.value = value;

            if (!hasLoadedTextures)
            {
                for (int i = 0; i < numberTextures.Length; i++)
                {
                    numberTextures[i] = new Texture2D($"Images/Money/{i}.png");
                }
                hasLoadedTextures = true;
            }

            this.isTotalMoney = isTotalMoney;
        }

        /// <summary>
        /// Gets the visual height of the notification in terms of the screen's height
        /// </summary>
        public float GetHeight(float aspectRatio)
        {
            return baseSize * heightScale * aspectRatio;
        }

        /// <summary>
        /// Updates the life time and height of a notification
        /// </summary>
        /// <param name="deltaTime">The time in seconds between the last call to this function</param>
        /// <returns>Whether the object is alive</returns>
        public bool Update(float deltaTime)
        {
            timeAlive += deltaTime;
            if (timeAlive > visibleDuration && timeAlive < shrinkingDuration + visibleDuration)
            {
                heightScale = 1 - (timeAlive - visibleDuration) / (shrinkingDuration);
            }

            return timeAlive < visibleDuration + shrinkingDuration;
        }

        /// <summary>
        /// Draws the value of the notification at a given height on the screen
        /// </summary>
        /// <param name="yPos"></param>
        /// <param name="uiShader"></param>
        /// <param name="aspectRatio"></param>
        public void Draw(float yPos, UIShader uiShader, float aspectRatio)
        {
            if (timeAlive >= visibleDuration)
                return;

            int drawValue = Math.Abs(value); // The number that needs to be written still
            float xPos = 1 - baseSize / 2.0f;

            // Draw a 0 if this is the player's total money display
            if (value == 0 && isTotalMoney)
            {
                uiShader.Draw(numberTextures[0], new Vector2(xPos, yPos), baseSize, aspectRatio);
                return;
            }

            while (true)
            {
                if (drawValue == 0)
                    break;

                int firstDigit = drawValue % 10;
                drawValue = drawValue / 10; // cut of last digit
                uiShader.Draw(numberTextures[firstDigit], new Vector2(xPos, yPos), baseSize, aspectRatio);
                xPos -= baseSize + margin;
            }

            if (value >= 0 && !isTotalMoney)
            {
                uiShader.Draw(plusTexture, new Vector2(xPos, yPos), baseSize, aspectRatio);
            }
            else
            {
                uiShader.Draw(minusTexture, new Vector2(xPos, yPos), baseSize, aspectRatio);
            }
        }

        public void SetValue(int value)
        {
            this.value = value;
        }
    }
}
