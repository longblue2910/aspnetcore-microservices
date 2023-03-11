using AutoMapper;
using Basket.API.Entities;
using Basket.API.Repositories.Interfaces;
using EventBus.Messages.IntegrationEvents.Events;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
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
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IMapper _mapper;

        public BasketController(IBasketRepository repository, IPublishEndpoint publishEndpoint, IMapper mapper)
        {
            _repository = repository;
            _publishEndpoint = publishEndpoint;
            _mapper = mapper;
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

        [Route("[action]")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> CheckOut([FromBody] BasketCheckout basketCheckout)
        {
            var basket = await _repository.GetBasketByUserName(basketCheckout.UserName);
            if (basket == null) return NotFound();
           
            //Publish checkout event to Eventbus Message
            var eventMessage = _mapper.Map<BasketCheckoutEvent>(basketCheckout);
            eventMessage.TotalPrice = basket.TotalPrice;
            await _publishEndpoint.Publish(eventMessage); 

            //Remove the basket
            await _repository.DeleteBasketFromUserName(basketCheckout.UserName);

            return Accepted();

        }
    }
}
