var builder = WebApplication.CreateBuilder(args);
var assembly = typeof(Program).Assembly;
// Adding Services to the container.

builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(assembly);
    config.AddOpenBehavior(typeof(ValidationBehavior<,>));
    config.AddOpenBehavior(typeof(LoggingBehavior<,>));
});
builder.Services.AddValidatorsFromAssembly(assembly);
builder.Services.AddCarter();
builder.Services.AddMarten(opts =>
    {
        opts.Connection(builder.Configuration.GetConnectionString("Database") ?? string.Empty);
    })
    .UseLightweightSessions();

builder.Services.AddExceptionHandler<CustomExceptionHandler>();

var app = builder.Build();

// Configure HTTP Request Pipeline

app.MapCarter();
app.UseExceptionHandler(_ => {});
app.Run();
