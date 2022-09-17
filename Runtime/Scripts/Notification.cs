using System;

namespace Volorf.VRNotifications
{
    [Serializable]
    public struct Notification
    {
        public string Message;
        public NotificationType Type;

        public Notification(string msg, NotificationType type, float dur)
        {
            Message = msg;
            Type = type;
        }
    }
}
