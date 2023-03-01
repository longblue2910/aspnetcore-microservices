using Shared.Services.Email;

namespace Contracts.Services
{
    public interface IEmailService 
    {
        void SendEmailAsync(MailRequest request, CancellationToken cancellationToken = new CancellationToken());
    }
}
