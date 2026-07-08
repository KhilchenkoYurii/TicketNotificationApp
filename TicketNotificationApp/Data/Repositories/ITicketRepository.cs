using TicketNotificationApp.Data.Models;

namespace TicketNotificationApp.Data.Repositories
{
    public interface ITicketRepository
    {
        Ticket AddTicket(Ticket ticket);

        Ticket? GetTicket(Guid id);

        void AddNotifications(IEnumerable<Notification> notifications);

        Notification? GetNotification(Guid id);

        List<Notification> GetNotificationsByTicket(Guid ticketId);

        List<Notification> GetFailedNotifications();

        void UpdateNotification(Notification notification);
    }
}
