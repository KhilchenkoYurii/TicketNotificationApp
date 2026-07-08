using TicketNotificationApp.Data.Models;

namespace TicketNotificationApp.Services
{
    public class NotificationStateLogic
    {
        private static readonly Dictionary<NotificationStatus, NotificationStatus[]> AllowedTransitions = new()
        {
            [NotificationStatus.Pending] = new[] { NotificationStatus.InProgress },
            [NotificationStatus.InProgress] = new[] { NotificationStatus.Sent, NotificationStatus.Failed },
            [NotificationStatus.Failed] = new[] { NotificationStatus.InProgress },
            [NotificationStatus.Sent] = Array.Empty<NotificationStatus>()
        };

        public static bool CanTransition(NotificationStatus from, NotificationStatus to)
        {
           return AllowedTransitions.TryGetValue(from, out var allowed) && allowed.Contains(to);
        }

        public static bool CanRetry(Notification notification, int maxAttempts = 3)
        {
            return notification.Status == NotificationStatus.Failed && notification.Attempts < maxAttempts;
        }
    }
}
