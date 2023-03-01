using Basket.API.Entities;
using Basket.API.Repositories.Interfaces;
using Contracts.Common.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using ILogger = Serilog.ILogger;

namespace Basket.API.Repositories
{
    public class BasketRepository : IBasketRepository
    {
        private readonly IDistributedCache _redisCacheService;
        private readonly ISerializeService _serializeService;
        private readonly ILogger _logger;

        public BasketRepository(IDistributedCache redisCacheService, ISerializeService serializeService, ILogger logger)
        {
            _redisCacheService = redisCacheService;
            _serializeService = serializeService;
            _logger = logger;
        }

        public async Task<bool> DeleteBasketFromUserName(string username)
        {
            _logger.Information($"BEGIN: DeleteBasketFromUserName {username}");


            try
            {
                await _redisCacheService.RemoveAsync(username);

                _logger.Information($"END: DeleteBasketFromUserName {username}");

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error($"DeleteBasketFromUserName {ex.Message}");
                throw;
            }
        }

        public async Task<Cart> GetBasketByUserName(string username)
        {
            _logger.Information($"BEGIN: GetBasketByUserName {username}");

            var basket = await _redisCacheService.GetStringAsync(username);

            _logger.Information($"END: GetBasketByUserName {username}");


            return string.IsNullOrEmpty(basket) ? null : _serializeService.Deserialize<Cart>(basket);
        }

        public async Task<Cart> UpdateBasket(Cart cart, DistributedCacheEntryOptions options = null)
        {
            _logger.Information($"BEGIN: UpdateBasket {cart.Username}");

            if (options != null)
            {
                await _redisCacheService.SetStringAsync(cart.Username,
                    _serializeService.Serialize(cart), options);
            }
            else
            {
                await _redisCacheService.SetStringAsync(cart.Username,
                    _serializeService.Serialize(cart));
            }

            _logger.Information($"END: UpdateBasket {cart.Username}");

            return await GetBasketByUserName(cart.Username);
        }
    }
}
