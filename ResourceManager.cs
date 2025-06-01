using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Voxel_Project
{
    /// <summary>
    /// Manages displaying a resource count and updates to that resource
    /// </summary>
    internal class ResourceManager
    {
        public enum Color
        {
            yellow,
            brown,

            none
        }

        Color color;
        bool isLeftAligned;
        int resourceCount = 0;
        List<ResourceNotification> notifications = new List<ResourceNotification>();
        ResourceNotification totalDisplayNotification;
        static Texture2D[] backgrounds = new Texture2D[(int)Color.none + 1];

        public ResourceManager(Color color, bool isLeftAligned)
        {
            totalDisplayNotification = new ResourceNotification(resourceCount, true);
            this.color = color;
            this.isLeftAligned = isLeftAligned;

            // Load images
            if (backgrounds[0] == null)
            {
                string[] names = Enum.GetNames<Color>();
                for (int i = 0; i < names.Length; i++)
                {
                    backgrounds[i] = new Texture2D($"Images/Resource/{names[i]}.png");
                }
            }
        }

        /// <summary>
        /// Updates notifications
        /// </summary>
        public void Update(float deltaTime)
        {
            List<ResourceNotification> toDelete = new List<ResourceNotification>();

            foreach (ResourceNotification notification in notifications)
            {
                if (!notification.Update(deltaTime))
                {
                    toDelete.Add(notification);
                }
            }

            foreach (ResourceNotification notification in toDelete)
            {
                notifications.Remove(notification);
            }
        }

        public void Draw(UIShader uiShader, float aspectRatio)
        {
             float yPos = 1.0f - totalDisplayNotification.GetHeight(aspectRatio) / 2.0f - 0.01f;

            totalDisplayNotification.Draw(yPos, uiShader, aspectRatio, backgrounds[(int)color], isLeftAligned);
            yPos -= totalDisplayNotification.GetHeight(aspectRatio);

            foreach (ResourceNotification notification in notifications)
            {
                notification.Draw(yPos, uiShader, aspectRatio, backgrounds[(int)color], isLeftAligned);
                yPos -= notification.GetHeight(aspectRatio);
            }
        }

        public void ChangeResource(int value)
        {
            notifications.Add(new ResourceNotification(value));
            resourceCount += value;
            totalDisplayNotification.SetValue(resourceCount);
        }
    }
}
