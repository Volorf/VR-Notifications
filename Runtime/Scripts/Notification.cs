using System;

namespace Volorf.VRNotifications
{
    [Serializable]
    public struct Notification
    {
        public readonly string Message;
        public readonly NotificationType Type;

        public Notification(string msg, NotificationType type, float dur)
        {
            Message = msg;
            Type = type;
        }
    }
}
