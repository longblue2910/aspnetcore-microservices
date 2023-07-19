using Common.Logging;
using Contracts.Saga.OrderManager;
using Saga.Orchestrator.HttpRepository;
using Saga.Orchestrator.HttpRepository.Interfaces;
using Saga.Orchestrator.OrderManager;
using Saga.Orchestrator.Services;
using Saga.Orchestrator.Services.Interfaces;
using Shared.Configurations;
using Shared.DTOs.Baskets;

namespace Saga.Orchestrator.Extensions
{
    public static class ServiceExtensions
    {
        internal static IServiceCollection AddConfigurationSettings(this IServiceCollection services,
            IConfiguration configuration)
        {
            var eventBusSettings = configuration.GetSection(nameof(EventBusSettings))
                .Get<EventBusSettings>();
            services.AddSingleton(eventBusSettings);

            var cacheSettings = configuration.GetSection(nameof(CacheSettings))
                .Get<CacheSettings>();
            services.AddSingleton(cacheSettings);

            var grpcSettings = configuration.GetSection(nameof(GrpcSettings))
                .Get<GrpcSettings>();
            services.AddSingleton(grpcSettings);

            var backgroundJobSettings = configuration.GetSection(nameof(BackgroundJobSettings))
                .Get<BackgroundJobSettings>();
            services.AddSingleton(backgroundJobSettings);

            return services;
        }

        public static IServiceCollection ConfigureService(this IServiceCollection services)
            => services.AddTransient<ICheckoutSagaService, CheckoutSagaService>()
                       .AddTransient<ISagaOrderManager<BasketCheckoutDto, OrderResponse>, SagaOrderManager>()
                       .AddTransient<LoggingDelegatingHandler>();

        public static IServiceCollection ConfigureHttpRepository(this IServiceCollection services)
            => services.AddScoped<IOrderHttpRepository, OrderHttpRepository>()
                       .AddScoped<IBasketHttpRepository, BasketHttpRepository>()
                       .AddScoped<IInventoryHttpRepository, InventoryHttpRepository>();

        public static void ConfigureHttpClients(this IServiceCollection services)
        {
            ConfigureOrderHttpClients(services);
            ConfigureBasketHttpClients(services);
            ConfigureInventoryHttpClients(services);
        }

        private static void ConfigureOrderHttpClients(this IServiceCollection services)
        {
            services.AddHttpClient<IOrderHttpRepository, OrderHttpRepository>("OrdersAPI", (sp, cl) =>
            {
                cl.BaseAddress = new Uri("http://localhost:5005/api/v1/");
            }).AddHttpMessageHandler<LoggingDelegatingHandler>();

            services.AddScoped(sp => sp.GetService<IHttpClientFactory>()
            .CreateClient("OrdersAPI"));
        }

        private static void ConfigureBasketHttpClients(this IServiceCollection services)
        {
            services.AddHttpClient<IBasketHttpRepository, BasketHttpRepository>("BasketsAPI", (sp, cl) =>
            {
                cl.BaseAddress = new Uri("http://localhost:5004/api/");
            }).AddHttpMessageHandler<LoggingDelegatingHandler>();

            services.AddScoped(sp => sp.GetService<IHttpClientFactory>()
            .CreateClient("BasketsAPI"));
        }

        private static void ConfigureInventoryHttpClients(this IServiceCollection services)
        {
            services.AddHttpClient<IInventoryHttpRepository, InventoryHttpRepository>("InventoryAPI", (sp, cl) =>
            {
                cl.BaseAddress = new Uri("http://localhost:5006/api/");
            }).AddHttpMessageHandler<LoggingDelegatingHandler>();

            services.AddScoped(sp => sp.GetService<IHttpClientFactory>()
            .CreateClient("InventoryAPI"));
        }
    }
}
