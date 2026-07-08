using System.ComponentModel.DataAnnotations;
using TicketNotificationApp.Data.Models;

namespace TicketNotificationApp.Data.DTO
{
    public class CreateTicketRequest
    {
        [Required]
        [MinLength(5, ErrorMessage = "Title must be at least 5 characters long.")]
        public string Title { get; set; } = string.Empty;

        [Required]
        public TicketPriority Priority { get; set; }
    }
}
