using System.Reflection;

using Consul;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Catalog.API.Extensions;

public static class ProgramExtensionsServices
{
    public static IServiceCollection AddMediator(
        this IServiceCollection services,
        Assembly assembly)
    {
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(assembly);
            config.AddOpenBehavior(typeof(ValidationBehavior<,>));
            config.AddOpenBehavior(typeof(LoggingBehavior<,>));
        });
        return services;
    }

    public static IServiceCollection AddServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddExceptionHandler<CustomExceptionHandler>();
        return services;
    }

    public static IServiceCollection AddAuth(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer("Bearer", opts =>
            {
                opts.Authority = configuration["IdentityServer:Authority"];
                opts.RequireHttpsMetadata = false;
                opts.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false,
                    ValidIssuer = "https://shopping.identityserver:6070",
                    ValidateIssuerSigningKey = false
                };
            });

        services.AddAuthorizationBuilder()
            .AddPolicy("CatalogPolicy", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim("client_id", "shopping-ms-api");
            });

        return services;
    }

    public static IServiceCollection AddHealChecks(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddHealthChecks()
            .AddNpgSql(configuration.GetConnectionString("Database") ?? string.Empty);
        return services;
    }

    public static IServiceCollection AddMinimalApiLibraries(
        this IServiceCollection services,
        IConfiguration configuration,
        Assembly assembly)
    {
        services.AddValidatorsFromAssembly(assembly);
        services.AddCarter();
        services.AddMarten(opts => { opts.Connection(configuration.GetConnectionString("Database") ?? string.Empty); })
            .UseLightweightSessions();

        return services;
    }
}
