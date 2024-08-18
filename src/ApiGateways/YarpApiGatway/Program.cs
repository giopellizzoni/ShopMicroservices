using Common.Logging;

using HealthChecks.UI.Client;

using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.RateLimiting;

using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog(SeriLogger.Configure);

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddRateLimiter(rateLimiterOptions =>
{
    rateLimiterOptions.AddFixedWindowLimiter("fixed", options =>
    {
        options.Window = TimeSpan.FromSeconds(10);
        options.PermitLimit = 5;
    });
});

builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseRateLimiter();
app.MapReverseProxy();
app.UseHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapHealthChecks("/health", new HealthCheckOptions());

app.Run();
