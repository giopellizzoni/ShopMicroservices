using Common.Logging;

using HealthChecks.UI.Client;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;

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

var authority = builder.Configuration["IdentityServer:Authority"];
Console.WriteLine($"AUTHORITY {authority}");
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer("Bearer", opts =>
    {
        opts.Authority = authority;
        opts.RequireHttpsMetadata = false;
        opts.TokenValidationParameters.ValidateAudience = false;
    });

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("CatalogPolicy", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("client_id", "shopping-ms-api");
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

app.UseAuthentication();
app.UseAuthorization();

app.MapCarter();
app.UseExceptionHandler(_ => {});
app.UseHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
app.Run();
