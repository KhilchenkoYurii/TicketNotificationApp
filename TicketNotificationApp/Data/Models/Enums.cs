namespace TicketNotificationApp.Data.Models
{
    public enum TicketPriority
    {
        Low,
        Medium,
        High
    }

    public enum NotificationChannel
    {
        Email,
        Sms,
        Push
    }

    public enum NotificationStatus
    {
        Pending,
        InProgress,
        Sent,
        Failed
    }
}
