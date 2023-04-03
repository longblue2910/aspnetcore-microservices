using Shared.Configurations;
using System.Text;

namespace Basket.API.Service
{
    public class EmailTemplateService
    {
        protected readonly BackgroundJobSettings BackgroundJobSettings;
        public EmailTemplateService(BackgroundJobSettings settings)
        {
            BackgroundJobSettings = settings;
        }
        private static readonly string _baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        private static readonly string _tmpFolder = Path.Combine(_baseDirectory, "EmailTemplates");

        protected string ReadEmailTemplateContent(string emailTemplateName, string format = "html")
        {
            var filePath = Path.Combine(_tmpFolder, emailTemplateName + "." + format);
            using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var sr = new StreamReader(fs, Encoding.Default);
            var emailText = sr.ReadToEnd();
            sr.Close();

            return emailText;
        }
    }
}
