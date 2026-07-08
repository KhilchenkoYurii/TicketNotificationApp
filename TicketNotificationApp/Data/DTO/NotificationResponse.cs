using TicketNotificationApp.Data.Models;

namespace TicketNotificationApp.Data.DTO
{
    public class NotificationResponse
    {
        public Guid Id { get; set; }

        public Guid TicketId { get; set; }

        public NotificationChannel Channel { get; set; }

        public NotificationStatus Status { get; set; }

        public int Attempts { get; set; }

        public string? LastError { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public static NotificationResponse FromModel(Notification n) => new()
        {
            Id = n.Id,
            TicketId = n.TicketId,
            Channel = n.Channel,
            Status = n.Status,
            Attempts = n.Attempts,
            LastError = n.LastError,
            CreatedAt = n.CreatedAt
        };
    }
}
