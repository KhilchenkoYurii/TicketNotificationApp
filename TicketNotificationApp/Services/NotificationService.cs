using TicketNotificationApp.Data.Models;
using TicketNotificationApp.Data.Repositories;
using TicketNotificationApp.Gateway;

namespace TicketNotificationApp.Services
{
    public class NotificationService : INotificationService
    {
        private const int MaxAttempts = 3;

        private readonly ITicketRepository _repository;

        private readonly INotificationGateway _gateway;

        public NotificationService(ITicketRepository repository, INotificationGateway gateway)
        {
            _repository = repository;
            _gateway = gateway;
        }

        public async Task<Notification> ProcessAsync(Notification notification, CancellationToken cancellationToken = default)
        {
            // Sent is terminal — never touch it again.
            if (notification.Status == NotificationStatus.Sent)
            {
                return notification;
            }

            // Failed notifications that exhausted their attempts are not eligible.
            if (notification.Status == NotificationStatus.Failed && !NotificationStateLogic.CanRetry(notification, MaxAttempts))
            {
                return notification;
            }

            if (!NotificationStateLogic.CanTransition(notification.Status, NotificationStatus.InProgress))
            {
                throw new InvalidOperationException(
                    $"Cannot transition notification {notification.Id} from {notification.Status} to {NotificationStatus.InProgress}.");
            }

            notification.Status = NotificationStatus.InProgress;

            _repository.UpdateNotification(notification);

            try
            {
                var success = await _gateway.SendAsync(notification, cancellationToken);

                if (success)
                {
                    notification.Status = NotificationStatus.Sent;
                    notification.LastError = null;
                }
                else
                {
                    notification.Attempts++;
                    notification.Status = NotificationStatus.Failed;
                    notification.LastError = "Gateway reported delivery failure.";
                }
            }
            catch (Exception ex)
            {
                notification.Attempts++;
                notification.Status = NotificationStatus.Failed;
                notification.LastError = ex.Message;
            }

            _repository.UpdateNotification(notification);

            return notification;
        }

        public async Task<List<Notification>> NotifyTicketAsync(Guid ticketId, CancellationToken cancellationToken = default)
        {
            var pending = _repository.GetNotificationsByTicket(ticketId)
                .Where(n => n.Status == NotificationStatus.Pending)
                .ToList();

            var results = new List<Notification>();

            foreach (var notification in pending)
            {
                results.Add(await ProcessAsync(notification, cancellationToken));
            }

            return results;
        }

        public async Task<List<Notification>> RetryFailedAsync(CancellationToken cancellationToken = default)
        {
            var retryable = _repository.GetFailedNotifications()
                .Where(n => NotificationStateLogic.CanRetry(n, MaxAttempts))
                .ToList();

            var results = new List<Notification>();

            foreach (var notification in retryable)
            {
                results.Add(await ProcessAsync(notification, cancellationToken));
            }

            return results;
        }
    }
}
