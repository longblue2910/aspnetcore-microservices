using Basket.API.Entities;
using Basket.API.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Caching.Distributed;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace Basket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        private readonly IBasketRepository _repository;

        public BasketController(IBasketRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("{username}", Name = "GetBassket")]
        [ProducesResponseType(typeof(Cart), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Cart>> GetBasketByUserName([Required] string username)
        {
            var result = await _repository.GetBasketByUserName(username);

            return Ok(result ?? new Entities.Cart());
        }

        [HttpPost(Name = "UpdateBasket")]
        [ProducesResponseType(typeof(Cart), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Cart>> UpdateBasket([Required] Cart cart)
        {
            var options = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(DateTime.UtcNow.AddHours(1))
                .SetSlidingExpiration(TimeSpan.FromMinutes(5));

            var result = await _repository.UpdateBasket(cart, options);

            return Ok(result);
        }

        [HttpDelete("{username}", Name = "DeleteBassket")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<bool>> DeleteBasket([Required] string username)
        {
            var result = await _repository.DeleteBasketFromUserName(username);

            return Ok(result);
        }
    }
}
