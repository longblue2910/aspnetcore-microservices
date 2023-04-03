using Contracts.ScheduledJobs;
using Contracts.Services;
using Hangfire.API.Services;
using Hangfire.API.Services.Interfaces;
using Infrastructure.Configurations;
using Infrastructure.ScheduledJobs;
using Infrastructure.Services;
using Shared.Configurations;

namespace Hangfire.API.Extensions
{
    public static class ServiceExtensions
    {
        internal static IServiceCollection AddConfigurationSettings(this IServiceCollection services,
           IConfiguration configuration)
        {
            var hangfireSettings = configuration.GetSection(nameof(HangfireSettings))
                .Get<HangfireSettings>();
            services.AddSingleton(hangfireSettings);

            var sMTPEmailSetting = configuration.GetSection(nameof(SMTPEmailSetting))
                .Get<SMTPEmailSetting>();
            services.AddSingleton(sMTPEmailSetting);

            return services;
        }

        public static IServiceCollection ConfigureService(this IServiceCollection services) 
        {
            services.AddTransient<IScheduledJobService, HangfireService>()
                    .AddScoped<ISmtpEmailService, SmtpEmailService>()
                    .AddTransient<IBackgroundJobService, BackgroundJobService>();

            return services;
        }
    }
}
