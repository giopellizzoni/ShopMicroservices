
using Common.Logging;

using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog(SeriLogger.Configure);

// Add services to the container.
builder.Services.AddRazorPages();
var address = builder.Configuration["ApiSettings:GatewayAddress"]!;

builder.Services.AddTransient<LoggingDelegatingHandler>();

builder.Services.AddRefitClient<ICatalogService>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri(address))
    .AddHttpMessageHandler<LoggingDelegatingHandler>();

builder.Services.AddRefitClient<IBasketService>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri(address))
    .AddHttpMessageHandler<LoggingDelegatingHandler>();

builder.Services.AddRefitClient<IOrderingService>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri(address))
    .AddHttpMessageHandler<LoggingDelegatingHandler>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
