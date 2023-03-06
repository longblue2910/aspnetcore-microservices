using AutoMapper;
using Contracts.Services;
using MediatR;
using Ordering.Application.Common.Interfaces;
using Ordering.Domain.Entities;
using Shared.SeedWork;
using Shared.Services.Email;
using Serilog;

namespace Ordering.Application.Features.V1.Orders
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, ApiResult<long>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;
        private readonly ISmtpEmailService _emailService;
        private readonly ILogger _logger;

        public CreateOrderCommandHandler(IOrderRepository orderRepository, IMapper mapper, ISmtpEmailService emailService, ILogger logger)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
            _emailService = emailService;
            _logger = logger;
        }

        private const string MethodName = "CreateOrderCommandHandler";

        public async Task<ApiResult<long>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            _logger.Information($"BEGIN: {MethodName} - Username: {request.Username}");
            var orderEntity = _mapper.Map<Order>(request);
            var addOrder = await _orderRepository.CreateOrderAsync(orderEntity);
            await _orderRepository.SaveChangesAsync();
            _logger.Information($"Order {addOrder.Id} is successfully created.");

            await SendEmailAsync(addOrder, cancellationToken);

            _logger.Information($"END: {MethodName} - Username: {request.Username}");

            return new ApiSuccessResult<long>(addOrder.Id);
        }

        private async Task SendEmailAsync(Order order, CancellationToken cancellationToken)
        {
            var emailRequest = new MailRequest
            {
                ToAddress = order.EmailAddress,
                Body = "Order was created",
                Subject = "Order was created"
            };

            try
            {
                await _emailService.SendEmailAsync(emailRequest, cancellationToken);
                _logger.Information($"Sent created order to email {order.EmailAddress}");
            }
            catch (Exception ex)
            {
                _logger.Error($"Order {order.Id} failed due to an error with the email service: {ex.Message}");
            }
        }
    }
}
