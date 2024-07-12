var builder = WebApplication.CreateBuilder(args);

// Adding Services to the container.
builder.Services.AddCarter();
builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(typeof(Program).Assembly);
});

builder.Services.AddMarten(opts =>
{
    opts.Connection(builder.Configuration.GetConnectionString("Database") ?? string.Empty);
})
.UseLightweightSessions();

var app = builder.Build();

// Configure HTTP Request Pipeline

app.MapCarter();

app.Run();
