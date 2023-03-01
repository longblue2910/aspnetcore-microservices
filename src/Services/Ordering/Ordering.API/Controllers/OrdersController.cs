using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ordering.Application.Common.Models;
using Ordering.Application.Features.V1.Orders;
using System.ComponentModel.DataAnnotations;

namespace Ordering.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrdersController(IMediator mediator)
        {
            _mediator = mediator;
        }


        private static class RouteNames
        {
            public const string GetOrders = nameof(GetOrders);
        }

        [HttpGet("{username}")]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrdersByUserName([Required] string username)
        {
            var query = new GetOrdersQuery(username);
            var result = await _mediator.Send(query);

            return Ok(result);
        }
    }
}
