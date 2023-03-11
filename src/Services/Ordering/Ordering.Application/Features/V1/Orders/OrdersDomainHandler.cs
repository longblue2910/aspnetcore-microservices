using AutoMapper;
using Contracts.Services;
using Infrastructure.Services;
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
        private readonly ITemplateEmailService _templateService;

        public OrdersDomainHandler(ILogger logger, ISmtpEmailService emailService, ITemplateEmailService templateService)
        {
            _logger = logger;
            _emailService = emailService;
            _templateService = templateService;
        }

        public Task Handle(OrderDeletedEvent notification, CancellationToken cancellationToken)
        {
            _logger.Information("Ordering Domain Event: {DomainEvent}", notification.GetType().Name);
            return Task.CompletedTask;
        }

        public async Task Handle(OrderCreatedEvent notification, CancellationToken cancellationToken)
        {
            _logger.Information("Ordering Domain Event: {DomainEvent}", notification.GetType().Name);

            var mailModel = new MailModel
            {
                DocumentNo = notification.DocumentNo,
                FirstName = notification.FirstName,
                ShippingAddress = notification.ShippingAddress,
                EmailAddress = notification.EmailAddress,
                InvoiceAddress = notification.InvoiceAddress,
                Url = notification.Url,
                LastName = notification.LastName,
                TotalPrice = notification.TotalPrice,
                UserName = notification.UserName,
            };
            var emailBody = await _templateService.GetTemplateHtmlAsStringAsync("EmailTemplate", mailModel);

            var emailRequest = new MailRequest
            {
                ToAddress = notification.EmailAddress,
                Body = emailBody,
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
