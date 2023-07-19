using Saga.Orchestrator.HttpRepository.Interfaces;
using Shared.DTOs.Baskets;

namespace Saga.Orchestrator.HttpRepository
{
    public class BasketHttpRepository : IBasketHttpRepository
    {
        private readonly HttpClient _client;

        public BasketHttpRepository(HttpClient client)
        {
            _client = client;
        }
        public async Task<bool> DeleteBasket(string username)
        {
            var response = await _client.DeleteAsync($"baskets/{username}");
            if (!response.EnsureSuccessStatusCode().IsSuccessStatusCode) 
                throw new Exception($"Delete basket for Username: {username} not success");

            var result = response.IsSuccessStatusCode;
            return result;
        }

        public async Task<CartDto> GetBasket(string username)
        {
            var cart = await _client.GetFromJsonAsync<CartDto>($"basket/{username}");
            if (cart == null || !cart.Items.Any()) 
                return new CartDto();

            return cart;
        }
    }
}
