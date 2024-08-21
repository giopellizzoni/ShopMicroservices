using System.IdentityModel.Tokens.Jwt;
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

    public static IServiceCollection AddHealthChecks(
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
