using Microsoft.AspNetCore.Mvc;
using TicketNotificationApp.Data.DTO;
using TicketNotificationApp.Data.Models;
using TicketNotificationApp.Data.Repositories;
using TicketNotificationApp.Services;


namespace TicketNotificationApp.Controllers
{
    [ApiController]
    [Route("api/tickets")]
    public class TicketsController : ControllerBase
    {
        private readonly ITicketRepository _repository;

        private readonly INotificationService _notificationService;

        public TicketsController(ITicketRepository repository, INotificationService notificationService)
        {
            _repository = repository;
            _notificationService = notificationService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(TicketResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult CreateTicket([FromBody] CreateTicketRequest request)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var ticket = new Ticket
            {
                Title = request.Title,
                Priority = request.Priority
            };

            _repository.AddTicket(ticket);

            var notifications = Enum.GetValues<NotificationChannel>()
                .Select(channel => new Notification
                {
                    TicketId = ticket.Id,
                    Channel = channel
                })
                .ToList();

            _repository.AddNotifications(notifications);

            var response = TicketResponse.FromModel(ticket, notifications);

            return CreatedAtAction(nameof(GetTicket), new { id = ticket.Id }, response);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(TicketResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetTicket(Guid id)
        {
            var ticket = _repository.GetTicket(id);

            if (ticket is null)
            {
                return NotFound();
            }

            var notifications = _repository.GetNotificationsByTicket(id);

            return Ok(TicketResponse.FromModel(ticket, notifications));
        }

        [HttpPost("{id:guid}/notify")]
        [ProducesResponseType(typeof(List<NotificationResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Notify(Guid id, CancellationToken cancellationToken)
        {
            var ticket = _repository.GetTicket(id);

            if (ticket is null)
            {
                return NotFound();
            }

            var results = await _notificationService.NotifyTicketAsync(id, cancellationToken);

            return Ok(results.Select(NotificationResponse.FromModel));
        }
    }
}
