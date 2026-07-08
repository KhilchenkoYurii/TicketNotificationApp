using TicketNotificationApp.Data.Models;

namespace TicketNotificationApp.Data.DTO
{
    public class TicketResponse
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public TicketPriority Priority { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public List<NotificationResponse> Notifications { get; set; } = new();

        public static TicketResponse FromModel(Ticket ticket, IEnumerable<Notification> notifications) => new()
        {
            Id = ticket.Id,
            Title = ticket.Title,
            Priority = ticket.Priority,
            CreatedAt = ticket.CreatedAt,
            Notifications = notifications.Select(NotificationResponse.FromModel).ToList()
        };
    }
}
