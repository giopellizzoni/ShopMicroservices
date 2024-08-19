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

var authority = builder.Configuration["IdentityServer:Authority"];

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer("Bearer", opts =>
    {
        opts.Authority = authority;
        opts.RequireHttpsMetadata = false;
        opts.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false
        };
    });

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("DiscountPolicy", policy => policy.RequireClaim("client_id", "shopping-ms-api"));


// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddDbContext<DiscountContext>(opts => opts.UseSqlite(builder.Configuration.GetConnectionString("Database")));
builder.Services.AddGrpcHealthChecks()
    .AddCheck("Health", () => HealthCheckResult.Healthy());

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.UseMigration();
// Configure the HTTP request pipeline.
app.MapGrpcService<DiscountService>();
app.MapGrpcHealthChecksService();

app.Run();
