var builder = WebApplication.CreateBuilder(args);

// Adding Services to the container.
builder.Services.AddCarter();
builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(typeof(Program).Assembly);
});

var app = builder.Build();

// Configure HTTP Request Pipeline

app.MapCarter();

app.Run();
