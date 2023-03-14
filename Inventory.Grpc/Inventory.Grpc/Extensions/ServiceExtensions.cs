using Inventory.Grpc.Repositories;
using Inventory.Grpc.Repositories.Interfaces;
using MongoDB.Driver;
using Shared.Configurations;

namespace Inventory.Grpc.Extensions
{
    public static class ServiceExtensions
    {
        internal static IServiceCollection AddConfigurationSettings(this IServiceCollection services,
            IConfiguration configuration)
        {
            var databaseSettings = configuration.GetSection(nameof(MongoDbSettings))
                .Get<MongoDbSettings>();

            services.AddSingleton(databaseSettings);
            return services;
        }

        private static string getMongoConectionString(this IServiceCollection services)
        {
            var settings = services.GetOptionMongoDbs<MongoDbSettings>(nameof(MongoDbSettings));
            if (settings == null || string.IsNullOrEmpty(settings.ConnectionString))
                throw new ArgumentNullException("DatabaseSettings is not configured.");

            var databaseName = settings.DatabaseName;
            var mongoDbConnectionString = settings.ConnectionString + "/" + databaseName + "?authSource=admin";

            return mongoDbConnectionString;
        }

        public static void ConfigureMongDbClient(this IServiceCollection services)
        {
            services.AddSingleton<IMongoClient>(
                new MongoClient(getMongoConectionString(services)))
                .AddScoped(x => x.GetService<IMongoClient>()?.StartSession());
        }

        public static void AddInfastructure(this IServiceCollection services)
        {
            services.AddScoped<IInventoryRepository, InventoryRepository>();
        }

        private static T GetOptionMongoDbs<T>(this IServiceCollection services, string sectionName) where T : new()
        {
            using var serviceProvider = services.BuildServiceProvider();
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            var section = configuration.GetSection(sectionName);
            var options = new T();
            section.Bind(options);

            return options;
        }
    }
}
