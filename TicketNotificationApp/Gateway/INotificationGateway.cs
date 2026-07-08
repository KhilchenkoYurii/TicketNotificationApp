using TicketNotificationApp.Data.Models;

namespace TicketNotificationApp.Gateway
{
    public interface INotificationGateway
    {
        Task<bool> SendAsync(Notification notification, CancellationToken cancellationToken = default);
    }
}
