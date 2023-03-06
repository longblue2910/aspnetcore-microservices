using Common.Logging;
using Contracts.Common.Interfaces;
using Infrastructure.Common;
using Ordering.API.Extensions;
using Ordering.Application;
using Ordering.Infrastructure;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog(Serilogger.Configure);

Log.Information("Start Ordering API up.");

try
{

    // Add services to the container.
    builder.Host.AddAppConfigurations();
    builder.Services.AddConfigurationSettings(builder.Configuration);
    builder.Services.AddApplicationServices();
    builder.Services.AddInfrastructureServices(builder.Configuration);
    //Configure Mass Transit
    builder.Services.ConfigureMasstransit();

    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddScoped<ISerializeService, SerializeService>();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    //Auto migration
    //app.MigrateDatabase<OrderContext>((context, _) =>
    //{
    //    OrderContextSeed.SeedOrderAsync(context, Log.Logger).Wait();
    //}).Run();

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();

}
catch (Exception ex)
{
    string type = ex.GetType().Name;

    if (type.Equals("StopTheHostException", StringComparison.Ordinal)) throw;

    Log.Fatal(ex, "Unhandled exception");
}
finally
{
    Log.Information("Shut down Ordering API Complete");
    Log.CloseAndFlush();
}
