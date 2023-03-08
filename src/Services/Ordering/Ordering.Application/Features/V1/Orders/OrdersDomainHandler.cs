using Contracts.Services;
using MediatR;
using Ordering.Domain.OrderAggregate.Events;
using Serilog;
using Shared.Services.Email;

namespace Ordering.Application.Features.V1.Orders
{
    public class OrdersDomainHandler :
        INotificationHandler<OrderCreatedEvent>,
        INotificationHandler<OrderDeletedEvent>
    {
        private readonly ILogger _logger;
        private readonly ISmtpEmailService _emailService;

        public OrdersDomainHandler(ILogger logger, ISmtpEmailService emailService)
        {
            _logger = logger;
            _emailService = emailService;
        }

        public Task Handle(OrderDeletedEvent notification, CancellationToken cancellationToken)
        {
            _logger.Information("Ordering Domain Event: {DomainEvent}", notification.GetType().Name);
            return Task.CompletedTask;
        }

        public async Task Handle(OrderCreatedEvent notification, CancellationToken cancellationToken)
        {
            _logger.Information("Ordering Domain Event: {DomainEvent}", notification.GetType().Name);
            var emailRequest = new MailRequest
            {
                ToAddress = notification.EmailAddress,
                Body = $"Your order detail." +
                       $"<p> Order Id: {notification.DocumentNo}</p>" +
                       $"<p> Total: {notification.TotalPrice}$</p>",
                Subject = $"Hello {notification.LastName}, your order was created."
            };

            try
            {
                await _emailService.SendEmailAsync(emailRequest, cancellationToken);
                _logger.Information($"Sent created order to email {notification.EmailAddress}");
            }
            catch (Exception ex)
            {
                _logger.Error($"Order {notification.DocumentNo} failed due to an error with the email service: {ex.Message}");
            }
        }
    }
}
