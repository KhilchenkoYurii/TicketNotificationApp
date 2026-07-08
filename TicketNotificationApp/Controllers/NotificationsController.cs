using Microsoft.AspNetCore.Mvc;
using TicketNotificationApp.Data.DTO;
using TicketNotificationApp.Services;

namespace TicketNotificationApp.Controllers
{
    [ApiController]
    [Route("api/notifications")]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpPost("retry")]
        [ProducesResponseType(typeof(List<NotificationResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> RetryFailed(CancellationToken cancellationToken)
        {
            var results = await _notificationService.RetryFailedAsync(cancellationToken);

            return Ok(results.Select(NotificationResponse.FromModel));
        }
    }
}
