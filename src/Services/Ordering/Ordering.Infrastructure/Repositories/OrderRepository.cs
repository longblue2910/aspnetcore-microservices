using Contracts.Common.Interfaces;
using Infrastructure.Common;
using Microsoft.EntityFrameworkCore;
using Ordering.Application.Common.Interfaces;
using Ordering.Domain.Entities;
using Ordering.Infrastructure.Persistence;

namespace Ordering.Infrastructure.Repositories
{
    public class OrderRepository : RepositoryBase<Order, long, OrderContext>, IOrderRepository
    {
        public OrderRepository(OrderContext dbContext, IUnitOfWork<OrderContext> unitOfWork) : base(dbContext, unitOfWork)
        {
        }

        public async Task<Order> CreateOrderAsync(Order orderDto)
        {
            await CreateAsync(orderDto);
            return orderDto;
        }

        public Task<Order> GetOrderByDocumentNo(string documentNo)
            => FindByCondition(x => x.DocumentNo.Equals(documentNo)).FirstOrDefaultAsync();   


        public async Task<IEnumerable<Order>> GetOrdersByUsername(string username) =>
            await FindByCondition(x => x.UserName.Equals(username)).ToListAsync();

        public async Task<Order> UpdateOrderAsync(Order orderDto)
        {
            await UpdateAsync(orderDto);
            return orderDto;
        }
    }
}
