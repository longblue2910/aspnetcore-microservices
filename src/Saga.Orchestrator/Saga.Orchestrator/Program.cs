using Common.Logging;
using Saga.Orchestrator;
using Saga.Orchestrator.Extensions;
using Serilog;

Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateBootstrapLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog(Serilogger.Configure);

Log.Information($"Start {builder.Environment.ApplicationName} up");

try
{

    builder.Host.AddAppConfigurations();

    builder.Services.ConfigureService();
    builder.Services.ConfigureHttpRepository();
    builder.Services.ConfigureHttpClients();


    builder.Host.UseSerilog(Serilogger.Configure);
    // Add services to the container.

    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.Configure<RouteOptions>(options => 
        options.LowercaseUrls = true);
    builder.Services.AddAutoMapper(cfg => cfg.AddProfile(new MappingProfile()));


    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json",
                    $"{builder.Environment.ApplicationName} v1"));
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{

    Log.Fatal(ex, "Unhandled exception");
}
finally
{
    Log.Information("Shut down Basket API Complete");
    Log.CloseAndFlush();
}

