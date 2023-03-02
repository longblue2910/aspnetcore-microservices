using Contracts.Services;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.Services.Email;

namespace Ordering.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ISmtpEmailService _smtpEmailService;

        public OrderController(IMediator mediator, ISmtpEmailService smtpEmailService)
        {
            _mediator = mediator;
            _smtpEmailService = smtpEmailService;
        }

        [HttpGet("test-email")]
        public async Task<IActionResult> TestEmail()
        {
            var message = new MailRequest
            {
                Body = "<h1>Hello Long T</h1>",
                Subject = "TEST EMAIL",
                ToAddress = "jootinno1@gmail.com"
            };
            await _smtpEmailService.SendEmailAsync(message);

            return Ok();
        }
    }
}
