using Common.Logging;

using Discount.Grpc.Data;
using Discount.Grpc.Services;

using Grpc.Health.V1;

using Microsoft.AspNetCore.Authentication.JwtBearer;

using Serilog;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog(SeriLogger.Configure);

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddDbContext<DiscountContext>(opts => opts.UseSqlite(builder.Configuration.GetConnectionString("Database")));
builder.Services.AddGrpcHealthChecks()
    .AddCheck("Health", () => HealthCheckResult.Healthy());

var app = builder.Build();

app.UseMigration();
// Configure the HTTP request pipeline.
app.MapGrpcService<DiscountService>();
app.MapGrpcHealthChecksService();

app.Run();
