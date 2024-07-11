var builder = WebApplication.CreateBuilder(args);

// Adding Services to the container.


var app = builder.Build();

// Configure HTTP Request Pipeline

app.Run();
