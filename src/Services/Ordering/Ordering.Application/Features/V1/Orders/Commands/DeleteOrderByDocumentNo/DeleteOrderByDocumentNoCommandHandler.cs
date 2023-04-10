using MediatR;
using Ordering.Application.Common.Interfaces;
using Serilog;
using Shared.SeedWork;

namespace Ordering.Application.Features.V1.Orders
{
    public class DeleteOrderByDocumentNoCommandHandler : IRequestHandler<DeleteOrderByDocumentNoCommand, ApiResult<bool>>
    {
        private readonly IOrderRepository _repository;
        private readonly ILogger _logger;

        public DeleteOrderByDocumentNoCommandHandler(IOrderRepository repository, ILogger logger)
        {
            _repository = repository;
            _logger = logger;
        }
        public async Task<ApiResult<bool>> Handle(DeleteOrderByDocumentNoCommand request, CancellationToken cancellationToken)
        {
            var orderEntity = await _repository.GetOrderByDocumentNo(request.DocumentNo);
            if (orderEntity == null) return new ApiResult<bool>(false);

            _repository.Delete(orderEntity);
            orderEntity.DeletedOrder();

            await _repository.SaveChangesAsync();

            _logger.Information($"Order {orderEntity.Id} was successfully deletd.");

            return new ApiResult<bool>(true);
        }
    }
}
