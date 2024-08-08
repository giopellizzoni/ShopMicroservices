using Common.Logging;

using Ordering.API;
using Ordering.Application;
using Ordering.Infrastructure;
using Ordering.Infrastructure.Data.Extensions;

using Serilog;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Host.UseSerilog(SeriLogger.Configure);

builder.Services
    .AddApplicationServices(configuration)
    .AddInfrastructureServices(configuration)
    .AddApiServices(configuration);

var app = builder.Build();

app.UseApiServices();

if (app.Environment.IsDevelopment())
{
    await app.InitialiseDatabaseAsync();
}

app.Run();
