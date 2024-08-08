﻿
using Common.Logging;

using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog(SeriLogger.Configure);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddRefitClient<ICatalogService>()
    .ConfigureHttpClient(c =>
    {
        var address = builder.Configuration["ApiSettings:GatewayAddress"] ?? "";
        c.BaseAddress = new Uri(address);
    });
builder.Services.AddRefitClient<IBasketService>()
    .ConfigureHttpClient(c =>
    {
        var address = builder.Configuration["ApiSettings:GatewayAddress"] ?? "";
        c.BaseAddress = new Uri(address);
    });

builder.Services.AddRefitClient<IOrderingService>()
    .ConfigureHttpClient(c =>
    {
        var address = builder.Configuration["ApiSettings:GatewayAddress"] ?? "";
        c.BaseAddress = new Uri(address);
    });


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
