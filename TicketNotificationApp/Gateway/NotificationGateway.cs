using TicketNotificationApp.Data.Models;

namespace TicketNotificationApp.Gateway
{
    public class NotificationGateway : INotificationGateway
    {
        private static readonly Random Random = new();

        public Task<bool> SendAsync(Notification notification, CancellationToken cancellationToken = default)
        {
            var success = Random.Next(100) < 70;

            if (!success)
            {
                throw new InvalidOperationException(
                    $"Simulated delivery failure for channel {notification.Channel}.");
            }

            return Task.FromResult(true);
        }
    }
}
