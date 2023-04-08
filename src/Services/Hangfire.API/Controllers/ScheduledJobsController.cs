using Hangfire.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.ScheduledJob;
using System.ComponentModel.DataAnnotations;

namespace Hangfire.API.Controllers
{
    [Route("api/scheduled-jobs")]
    [ApiController]
    public class ScheduledJobsController : ControllerBase
    {
        private readonly IBackgroundJobService _jobService;

        public ScheduledJobsController(IBackgroundJobService jobService)
        {
            _jobService = jobService;
        }

        [HttpPost]
        [Route("send-mail-reminder-checkout-order")]
        public IActionResult SendReminderCheckoutOrderEmail([FromBody] ReminderCheckoutOrderDto model)
        {
            var jobId = _jobService.SendMailContent(model.email, model.subject, model.emailContent, model.enqueueAt);

            return Ok(jobId);
        }

        [HttpDelete]
        [Route("delete/jobId/{id}")]
        public IActionResult DeleteJobId([Required] string id)
        {
            var result = _jobService.ScheduledJob.Delete(id);

            return Ok(result);
        }
    }
}
