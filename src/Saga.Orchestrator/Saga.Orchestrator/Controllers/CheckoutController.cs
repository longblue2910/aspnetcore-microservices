using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Saga.Orchestrator.Services.Interfaces;
using Shared.DTOs.Baskets;
using Shared.DTOs.Product;
using System.ComponentModel.DataAnnotations;

namespace Saga.Orchestrator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckoutController : ControllerBase
    {
        private readonly ICheckoutSagaService _checkoutSagaService;

        public CheckoutController(ICheckoutSagaService checkoutSagaService)
        {
            _checkoutSagaService = checkoutSagaService;
        }


        [HttpPost]
        [Route("{username}")]
        public async Task<IActionResult> CheckOutOrder([Required] string username, 
            [FromBody] BasketCheckoutDto model)
        {
            var result = await _checkoutSagaService.CheckoutOrderAsync(username, model);
            return Accepted(result);
        }
    }
}
