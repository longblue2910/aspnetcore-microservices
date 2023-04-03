namespace Hangfire.API.Services.Interfaces
{
    public interface IBackgroundJobService
    {
        string SendMailContent(string email, string subject, string emailContent, DateTimeOffset enqueueAt);
    }
}
