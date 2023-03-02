using Ordering.Domain.Entities;
using Serilog;

namespace Ordering.Infrastructure.Persistence
{
    public static class OrderContextSeed
    {
        public static async Task SeedOrderAsync(OrderContext orderContext, ILogger logger)
        {
            if (!orderContext.Orders.Any())
            {
                orderContext.AddRange(getOrders());
                await orderContext.SaveChangesAsync();
                logger.Information("Seeded data for Order DB associated with context {DbContextName}", nameof(orderContext));
            }
        }

        private static IEnumerable<Order> getOrders()
        {
            return new List<Order>
            {
                new Order
                        {
                            UserName = "customer1", FirstName = "customer1", LastName = "customer",
                            EmailAddress = "customer1@gmail.com",
                            ShippingAddress = "HCM city", InvoiceAddress = "Viet Nam", TotalPrice = 500000
                        }
            };
        }
    }
}
