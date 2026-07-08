using System.Collections.Concurrent;
using TicketNotificationApp.Data.Models;

namespace TicketNotificationApp.Data.Repositories
{
    public class TicketRepository : ITicketRepository
    {
        private readonly ConcurrentDictionary<Guid, Ticket> _tickets = new();

        private readonly ConcurrentDictionary<Guid, Notification> _notifications = new();

        public Ticket AddTicket(Ticket ticket)
        {
            _tickets[ticket.Id] = ticket;
            return ticket;
        }

        public Ticket? GetTicket(Guid id)
        {
            return _tickets.TryGetValue(id, out var ticket) ? ticket : null;
        }

        public void AddNotifications(IEnumerable<Notification> notifications)
        {
            foreach (var notification in notifications)
            {
                _notifications[notification.Id] = notification;
            }
        }

        public Notification? GetNotification(Guid id)
        {
            return _notifications.TryGetValue(id, out var notification) ? notification : null;
        }

        public List<Notification> GetNotificationsByTicket(Guid ticketId)
        {
            return _notifications.Values
                    .Where(n => n.TicketId == ticketId)
                    .OrderBy(n => n.CreatedAt)
                    .ToList();
        }

        public List<Notification> GetFailedNotifications()
        {
            return _notifications.Values
                    .Where(n => n.Status == NotificationStatus.Failed)
                    .ToList();
        }
            
        public void UpdateNotification(Notification notification)
        {
            _notifications[notification.Id] = notification;
        }
    }
}
