using Shared.DTOs.Baskets;

namespace Saga.Orchestrator.Services.Interfaces
{
    public interface ICheckoutSagaService
    {
        Task<bool> CheckoutOrderAsync(string username, BasketCheckoutDto basketCheckout);
    }
}
