namespace TicketNotificationApp.Data.Models
{
    public class Ticket
    {
        public Guid Id { get; init; } = Guid.NewGuid();

        public required string Title { get; init; }

        public TicketPriority Priority { get; init; }

        public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
    }
}
