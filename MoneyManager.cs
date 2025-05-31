using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Voxel_Project
{
    internal class MoneyManager
    {
        int money = 0;
        List<MoneyNotification> notifications = new List<MoneyNotification>();
        MoneyNotification currentMoneyNotification;

        public MoneyManager()
        {
            currentMoneyNotification = new MoneyNotification(money, true);
        }

        /// <summary>
        /// Updates money notifications
        /// </summary>
        public void Update(float deltaTime)
        {
            List<MoneyNotification> toDelete = new List<MoneyNotification>();

            foreach (MoneyNotification notification in notifications)
            {
                if (!notification.Update(deltaTime))
                {
                    toDelete.Add(notification);
                }
            }

            foreach (MoneyNotification notification in toDelete)
            {
                notifications.Remove(notification);
            }
        }

        public void Draw(UIShader uiShader, float aspectRatio)
        {
             float yPos = 1.0f - currentMoneyNotification.GetHeight(aspectRatio) / 2.0f - 0.01f;

            currentMoneyNotification.Draw(yPos, uiShader, aspectRatio);
            yPos -= currentMoneyNotification.GetHeight(aspectRatio);

            foreach (MoneyNotification notification in notifications)
            {
                notification.Draw(yPos, uiShader, aspectRatio);
                yPos -= notification.GetHeight(aspectRatio);
            }
        }

        public void ChangeMoney(int value)
        {
            notifications.Add(new MoneyNotification(value));
            money += value;
            currentMoneyNotification.SetValue(money);
        }
    }
}
