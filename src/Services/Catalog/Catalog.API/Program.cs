using Catalog.API.Extensions;
using Common.Logging;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var assembly = typeof(Program).Assembly;

// Adding Services to the container.

builder.Host.UseSerilog(SeriLogger.Configure);

builder.Services
    .AddMediator(assembly)
    .AddServices(builder.Configuration)
    .AddMinimalApiLibraries(builder.Configuration, assembly)
    .AddHealthChecks(builder.Configuration);

if (builder.Environment.IsDevelopment())
{
    builder.Services.InitializeMartenWith<CatalogInitialData>();
}

var app = builder.Build();


app.MapCarter();
app.UseExceptionHandler(_ => {});
app.UseHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
app.Run();
