using Common.Logging;

using HealthChecks.UI.Client;

using Microsoft.AspNetCore.Diagnostics.HealthChecks;

using Serilog;

var builder = WebApplication.CreateBuilder(args);
var assembly = typeof(Program).Assembly;
// Adding Services to the container.

builder.Host.UseSerilog(SeriLogger.Configure);

builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(assembly);
    config.AddOpenBehavior(typeof(ValidationBehavior<,>));
    config.AddOpenBehavior(typeof(LoggingBehavior<,>));
});
builder.Services.AddValidatorsFromAssembly(assembly);
builder.Services.AddCarter();
builder.Services.AddMarten(opts =>
    {
        opts.Connection(builder.Configuration.GetConnectionString("Database") ?? string.Empty);
    })
    .UseLightweightSessions();

if (builder.Environment.IsDevelopment())
{
    builder.Services.InitializeMartenWith<CatalogInitialData>();
}

builder.Services.AddExceptionHandler<CustomExceptionHandler>();
builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("Database") ?? string.Empty);

var app = builder.Build();

// Configure HTTP Request Pipeline

app.MapCarter();
app.UseExceptionHandler(_ => {});
app.UseHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
app.Run();
