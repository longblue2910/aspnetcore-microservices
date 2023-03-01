namespace Contracts.Services
{
    public interface IEmailService<in T> where T : class
    {
        void SendEmailAsync(T request, CancellationToken cancellationToken = new CancellationToken());
    }
}
