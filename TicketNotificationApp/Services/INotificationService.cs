using TicketNotificationApp.Data.Models;

namespace TicketNotificationApp.Services
{
    public interface INotificationService
    {
        Task<Notification> ProcessAsync(Notification notification, CancellationToken cancellationToken = default);

        Task<List<Notification>> NotifyTicketAsync(Guid ticketId, CancellationToken cancellationToken = default);

        Task<List<Notification>> RetryFailedAsync(CancellationToken cancellationToken = default);
    }
}
