using Contracts.ScheduledJobs;

namespace Hangfire.API.Services.Interfaces
{
    public interface IBackgroundJobService
    {
        IScheduledJobService ScheduledJob { get; }
        string SendMailContent(string email, string subject, string emailContent, DateTimeOffset enqueueAt);
    }
}
