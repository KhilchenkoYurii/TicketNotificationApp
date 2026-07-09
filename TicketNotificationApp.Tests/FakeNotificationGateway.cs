using TicketNotificationApp.Data.Models;
using TicketNotificationApp.Gateway;

namespace TicketNotificationApp.Tests
{
    public class FakeNotificationGateway : INotificationGateway
    {
        private readonly Queue<Func<bool>> _behaviors;

        public int CallCount { get; private set; }

        public FakeNotificationGateway(params Func<bool>[] behaviors)
        {
            _behaviors = new Queue<Func<bool>>(behaviors.Length > 0
                ? behaviors
                : new Func<bool>[] { () => true });
        }

        public static FakeNotificationGateway AlwaysSucceeds() => new(() => true);

        public static FakeNotificationGateway AlwaysFails() => new(() => throw new InvalidOperationException("Simulated failure"));

        public Task<bool> SendAsync(Notification notification, CancellationToken cancellationToken = default)
        {
            CallCount++;

            var behavior = _behaviors.Count > 1 ? _behaviors.Dequeue() : _behaviors.Peek();

            var result = behavior();

            return Task.FromResult(result);
        }
    }
}
