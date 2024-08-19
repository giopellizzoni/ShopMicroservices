using Common.Logging;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

using Ordering.API;
using Ordering.Application;
using Ordering.Infrastructure;
using Ordering.Infrastructure.Data.Extensions;

using Serilog;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

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
            ValidateAudience = false,
            ValidIssuer = "https://shopping.identityserver:6070",

        };
        opts.RequireHttpsMetadata = true;
    });

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("OrderingPolicy", policy => policy.RequireClaim("client_id", "shopping-ms-api"));


builder.Services
    .AddApplicationServices(configuration)
    .AddInfrastructureServices(configuration)
    .AddApiServices(configuration);

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.UseApiServices();

if (app.Environment.IsDevelopment())
{
    await app.InitialiseDatabaseAsync();
}

app.Run();
