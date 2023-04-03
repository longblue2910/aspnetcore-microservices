
using Contracts.ScheduledJobs;
using Microsoft.AspNetCore.Mvc;
using ILogger = Serilog.ILogger;

namespace Hangfire.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WelcomeController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IScheduledJobService _jobService;

        public WelcomeController(ILogger logger, IScheduledJobService jobService)
        {
            _logger = logger;
            _jobService = jobService;
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult Welcome()
        {
            var jobId = _jobService.Enqueue(() => ResponseWelCome("Welcome API"));
            return Ok($"Job Id {jobId} - Enqueue Job");
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult DelayedWelcome()
        {
            int seconds = 5;
            var jobId = _jobService.Schedule(() => ResponseWelCome("Welcome API"), 
                TimeSpan.FromSeconds(seconds));
            return Ok($"Job Id {jobId} - Delayed Job");
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult WelcomeAt()
        {
            var enqueueAt = DateTimeOffset.UtcNow.AddSeconds(10);
            var jobId = _jobService.Schedule(() => ResponseWelCome("Welcome API"),
                            enqueueAt); 
            return Ok($"Job Id {jobId} - Schedule Job");
        }


        [HttpPost]
        [Route("[action]")]
        public IActionResult ConfirmedWelcome()
        {
            const int timeInSeconds = 5;
            var parentJobId = _jobService.Schedule(() => ResponseWelCome("Welcome API"),
                TimeSpan.FromSeconds(timeInSeconds));

            var jobId = _jobService.ContinueQueueWith(parentJobId, () => ResponseWelCome("Welcome message is sent."));
            return Ok($"Job Id {jobId} - Continue Job");
        }

        [NonAction]
        public void ResponseWelCome(string text) => _logger.Information(text);
    }
}
