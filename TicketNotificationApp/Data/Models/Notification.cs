namespace TicketNotificationApp.Data.Models
{
    public class Notification
    {
        public Guid Id { get; init; } = Guid.NewGuid();

        public required Guid TicketId { get; init; }

        public required NotificationChannel Channel { get; init; }

        public NotificationStatus Status { get; set; } = NotificationStatus.Pending;

        public int Attempts { get; set; } = 0;

        public string? LastError { get; set; }

        public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
    }
}
