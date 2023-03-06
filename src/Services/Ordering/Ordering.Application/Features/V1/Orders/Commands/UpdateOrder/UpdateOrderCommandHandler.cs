using AutoMapper;
using Contracts.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ordering.Application.Common.Exceptions;
using Ordering.Application.Common.Interfaces;
using Ordering.Application.Common.Models;
using Ordering.Domain.Entities;
using Serilog;
using Shared.SeedWork;

namespace Ordering.Application.Features.V1.Orders
{
    public class UpdateOrderCommandHandler : IRequestHandler<UpdateOrderCommand, ApiResult<OrderDto>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;
        private readonly ISmtpEmailService _emailService;
        private readonly ILogger _logger;

        public UpdateOrderCommandHandler(IOrderRepository orderRepository, IMapper mapper, ISmtpEmailService emailService,
                                         ILogger logger)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
            _emailService = emailService;
            _logger = logger;
        }

        private const string MethodName = "UpdateOrderCommandHandler";

        public async Task<ApiResult<OrderDto>> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
        {
            var orderEntity = await _orderRepository.GetByIdAsync(request.Id);
            if (orderEntity == null) throw new NotFoundException(nameof(Order), request.Id);

            _logger.Information($"BEGIN: {MethodName} - Username: {request.Id}");

            orderEntity = _mapper.Map(request, orderEntity);
            var updateOrder = await _orderRepository.UpdateOrderAsync(orderEntity);
            await _orderRepository.SaveChangesAsync();

            _logger.Information($"Order {request.Id} was successfully updated.");
            var result = _mapper.Map<OrderDto>(updateOrder);

            _logger.Information($"END: {MethodName} - Username: {request.Id}");

            return new ApiSuccessResult<OrderDto>(result);
        }
    }
}
