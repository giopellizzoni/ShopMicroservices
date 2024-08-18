using Common.Logging;
using HealthChecks.UI.Client;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Polly;
using Polly.Extensions.Http;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog(SeriLogger.Configure);

// Add services to the container.
builder.Services.AddRazorPages();
var address = builder.Configuration["ApiSettings:GatewayAddress"]!;

builder.Services.AddTransient<LoggingDelegatingHandler>();

builder.Services.AddRefitClient<ICatalogService>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri(address))
    .AddHttpMessageHandler<LoggingDelegatingHandler>()
    .AddPolicyHandler(GetRetryPolicy())
    .AddPolicyHandler(GetCircuitBreakerPolicy());

builder.Services.AddRefitClient<IBasketService>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri(address))
    .AddHttpMessageHandler<LoggingDelegatingHandler>()
    .AddPolicyHandler(GetRetryPolicy())
    .AddPolicyHandler(GetCircuitBreakerPolicy());

builder.Services.AddRefitClient<IOrderingService>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri(address))
    .AddHttpMessageHandler<LoggingDelegatingHandler>()
    .AddPolicyHandler(GetRetryPolicy())
    .AddPolicyHandler(GetCircuitBreakerPolicy());

builder.Services.AddHealthChecks()
    .AddUrlGroup(new Uri($"{address}/health"), "Yarp Api Gateway", HealthStatus.Degraded);

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
    })
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
    {
        options.Authority = "https://localhost:5010";
        options.ClientId = "shopping-ms-api";
        options.ClientSecret = "840C7CDA-1E6F-42E7-A29C-3D12FE965A6F";
        options.ResponseType = "code id_token";

        options.Scope.Add("address");
        options.Scope.Add("email");
        options.Scope.Add("movieAPI");
        options.Scope.Add("roles");
        options.ClaimActions.MapUniqueJsonKey("role", "role");

        options.SaveTokens = true;

        options.GetClaimsFromUserInfoEndpoint = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            NameClaimType = JwtClaimTypes.GivenName,
            RoleClaimType = JwtClaimTypes.Role
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.MapHealthChecks("/health", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();

IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .WaitAndRetryAsync(
            retryCount: 5,
            sleepDurationProvider: retryAttemp => TimeSpan.FromSeconds(Math.Pow(2, retryAttemp)),
            onRetry: (
                exception,
                retryCount,
                context) =>
            {
                Log.Error($"Retry {retryCount} of {context.PolicyKey} at {context.OperationKey}, due to {exception}.");
            }
        );
}

IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .CircuitBreakerAsync(
            handledEventsAllowedBeforeBreaking: 5,
            durationOfBreak: TimeSpan.FromSeconds(30)
        );
}

