using TicketNotificationApp.Data.Models;
using TicketNotificationApp.Data.Repositories;
using TicketNotificationApp.Services;

namespace TicketNotificationApp.Tests
{
    public class NotificationServiceTests
    {
        private static Notification NewNotification(Guid ticketId, NotificationStatus status = NotificationStatus.Pending, int attempts = 0)
        {
            return new Notification
            {
                TicketId = ticketId,
                Channel = NotificationChannel.Email,
                Status = status,
                Attempts = attempts
            };
        }

        [Fact]
        public async Task ProcessAsync_PendingNotification_TransitionsToSent_WhenGatewaySucceeds()
        {
            var repository = new TicketRepository();
            var gateway = FakeNotificationGateway.AlwaysSucceeds();
            var service = new NotificationService(repository, gateway);

            var notification = NewNotification(Guid.NewGuid());
            repository.AddNotifications(new[] { notification });

            var result = await service.ProcessAsync(notification);

            Assert.Equal(NotificationStatus.Sent, result.Status);
            Assert.Null(result.LastError);
            Assert.Equal(0, result.Attempts);
        }

        [Fact]
        public async Task ProcessAsync_PendingNotification_TransitionsToFailed_WhenGatewayThrows()
        {
            var repository = new TicketRepository();
            var gateway = FakeNotificationGateway.AlwaysFails();
            var service = new NotificationService(repository, gateway);

            var notification = NewNotification(Guid.NewGuid());
            repository.AddNotifications(new[] { notification });

            var result = await service.ProcessAsync(notification);

            Assert.Equal(NotificationStatus.Failed, result.Status);
            Assert.Equal(1, result.Attempts);
            Assert.NotNull(result.LastError);
        }

        [Fact]
        public void StateLogic_SentToFailed_IsForbidden()
        {
            var canTransition = NotificationStateLogic.CanTransition(NotificationStatus.Sent, NotificationStatus.Failed);

            Assert.False(canTransition);
        }

        [Fact]
        public void StateLogic_SentToInProgress_IsForbidden()
        {
            var canTransition = NotificationStateLogic.CanTransition(NotificationStatus.Sent, NotificationStatus.InProgress);

            Assert.False(canTransition);
        }

        [Fact]
        public void StateLogic_FailedToPending_IsForbidden()
        {
            var canTransition = NotificationStateLogic.CanTransition(NotificationStatus.Failed, NotificationStatus.Pending);

            Assert.False(canTransition);
        }

        [Fact]
        public async Task RetryFailedAsync_IncrementsAttempts_OnRepeatedFailure()
        {
            var repository = new TicketRepository();
            var gateway = FakeNotificationGateway.AlwaysFails();
            var service = new NotificationService(repository, gateway);

            var notification = NewNotification(Guid.NewGuid(), NotificationStatus.Failed, attempts: 1);
            repository.AddNotifications(new[] { notification });

            var results = await service.RetryFailedAsync();

            Assert.Single(results);
            Assert.Equal(2, results[0].Attempts);
            Assert.Equal(NotificationStatus.Failed, results[0].Status);
        }

        [Fact]
        public async Task RetryFailedAsync_DoesNotRetry_WhenAttemptsAlreadyAtLimit()
        {
            var repository = new TicketRepository();
            var gateway = FakeNotificationGateway.AlwaysFails();
            var service = new NotificationService(repository, gateway);

            var notification = NewNotification(Guid.NewGuid(), NotificationStatus.Failed, attempts: 3);
            repository.AddNotifications(new[] { notification });

            var results = await service.RetryFailedAsync();

            Assert.Empty(results);
            Assert.Equal(3, notification.Attempts);
            Assert.Equal(0, gateway.CallCount);
        }

        [Fact]
        public async Task RetryFailedAsync_OnlyRetriesFailedNotifications_IgnoresOtherStatuses()
        {
            var repository = new TicketRepository();
            var gateway = FakeNotificationGateway.AlwaysSucceeds();
            var service = new NotificationService(repository, gateway);

            var ticketId = Guid.NewGuid();
            var pending = NewNotification(ticketId, NotificationStatus.Pending);
            var inProgress = NewNotification(ticketId, NotificationStatus.InProgress);
            var sent = NewNotification(ticketId, NotificationStatus.Sent);
            var failed = NewNotification(ticketId, NotificationStatus.Failed, attempts: 1);

            repository.AddNotifications(new[] { pending, inProgress, sent, failed });

            var results = await service.RetryFailedAsync();

            Assert.Single(results);
            Assert.Equal(failed.Id, results[0].Id);
            Assert.Equal(NotificationStatus.Sent, results[0].Status);
        }

        [Fact]
        public async Task RetryFailedAsync_IgnoresAlreadySentNotifications()
        {
            var repository = new TicketRepository();
            var gateway = FakeNotificationGateway.AlwaysSucceeds();
            var service = new NotificationService(repository, gateway);

            var sent = NewNotification(Guid.NewGuid(), NotificationStatus.Sent);
            repository.AddNotifications(new[] { sent });

            var results = await service.RetryFailedAsync();

            Assert.Empty(results);
            Assert.Equal(0, gateway.CallCount);
        }

        [Fact]
        public async Task NotifyTicketAsync_OnlyProcessesPendingNotifications()
        {
            var repository = new TicketRepository();
            var gateway = FakeNotificationGateway.AlwaysSucceeds();
            var service = new NotificationService(repository, gateway);

            var ticketId = Guid.NewGuid();
            var pending = NewNotification(ticketId, NotificationStatus.Pending);
            var alreadySent = NewNotification(ticketId, NotificationStatus.Sent);
            var failed = NewNotification(ticketId, NotificationStatus.Failed, attempts: 1);

            repository.AddNotifications(new[] { pending, alreadySent, failed });

            var results = await service.NotifyTicketAsync(ticketId);

            Assert.Single(results);
            Assert.Equal(pending.Id, results[0].Id);
            Assert.Equal(NotificationStatus.Sent, results[0].Status);
        }

        [Fact]
        public async Task ProcessAsync_AlreadySentNotification_IsLeftUnchanged()
        {
            var repository = new TicketRepository();
            var gateway = FakeNotificationGateway.AlwaysFails();
            var service = new NotificationService(repository, gateway);

            var sent = NewNotification(Guid.NewGuid(), NotificationStatus.Sent);
            repository.AddNotifications(new[] { sent });

            var result = await service.ProcessAsync(sent);

            Assert.Equal(NotificationStatus.Sent, result.Status);
            Assert.Equal(0, gateway.CallCount);
        }
    }
}