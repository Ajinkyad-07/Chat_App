using WebSocketServer.Infrastructure.Extensions;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

// Add services
builder.Services.AddServices();

// Add services to the container.
builder.Services.AddControllers();

// Add Swagger services.
builder.Services.AddEndpointsApiExplorer(); // Required for minimal APIs
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Chat API",
        Version = "v1",
        Description = "An example API using Swagger",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Ajinkya Dhumale"
        }
    });

    // Optional: Add XML comments for better documentation
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

var firebaseConfig = builder.Configuration.GetSection("Firebase");
FirebaseApp.Create(new AppOptions
{
    Credential = GoogleCredential.FromFile(firebaseConfig["ServiceAccountKeyPath"])
});


var app = builder.Build();

// Enable WebSockets
app.UseWebSockets();

// WebSocket route
app.Use(async (context, next) =>
{
    await next();
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        options.RoutePrefix = string.Empty; // Serve Swagger at the app's root
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

await app.RunAsync();
