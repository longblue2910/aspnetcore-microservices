using Contracts.Configurations;
using Contracts.Services;
using Serilog;
using Shared.Services.Email;

namespace Infrastructure.Services
{
    public class SmtpEmailService : IEmailService<MailRequest>
    {
        private readonly ILogger _logger;
        private readonly IEmailSMTPSettings _setting;

        public SmtpEmailService(ILogger logger, IEmailSMTPSettings setting)
        {
            _logger = logger;
            _setting = setting;
        }

        public void SendEmailAsync(MailRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
