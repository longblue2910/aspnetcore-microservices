using Shared.DTOs.Baskets;

namespace Saga.Orchestrator.HttpRepository.Interfaces
{
    public interface IBasketHttpRepository
    {
        Task<CartDto> GetBasket(string username);
        Task<bool> DeleteBasket(string username);
    }
}
