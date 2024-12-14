using WebSocketServer.Infrastructure.Extensions;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using System.Net.WebSockets;
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;


var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

// Add services
builder.Services.AddServices();

// Add CORS services
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()  // Allow any origin
            .AllowAnyMethod()     // Allow any HTTP method (GET, POST, etc.)
            .AllowAnyHeader();    // Allow any header
    });
});

// Add services to the container.
builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNamingPolicy = null; // Disable camelCase
        }); ;

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

// Use CORS policy
app.UseCors("AllowAll");

// Enable WebSockets
app.UseWebSockets();
// Store active WebSocket connections
var connections = new ConcurrentDictionary<string, WebSocket>();
// WebSocket route
app.Use(async (context, next) =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        string userId = context.Request.Query["userId"];
        if (string.IsNullOrEmpty(userId))
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsync("User ID is required");
            return;
        }

        WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
        connections[userId] = webSocket;

        Console.WriteLine($"User connected: {userId}");

        await HandleWebSocketConnection(userId, webSocket, connections);
    }
    else
    {
        await next();
    }
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

app.MapGet("/", () => "WebSocket Server is running...");

await app.RunAsync();

async Task HandleWebSocketConnection(string userId, WebSocket webSocket, ConcurrentDictionary<string, WebSocket> connections)
{
    var buffer = new byte[1024 * 4];

    while (webSocket.State == WebSocketState.Open)
    {
        try
        {
            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            if (result.CloseStatus.HasValue)
            {
                connections.TryRemove(userId, out _);
                await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
                Console.WriteLine($"User disconnected: {userId}");
                break;
            }

            string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
            Console.WriteLine($"Message from {userId}: {message}");

            // Deserialize JSON message
            var incomingMessage = JsonSerializer.Deserialize<IncomingMessage>(message);
            if (incomingMessage != null)
            {
                string targetUserId = incomingMessage.UserId;

                if (connections.TryGetValue(targetUserId, out WebSocket targetWebSocket) && targetWebSocket.State == WebSocketState.Open)
                {
                    // Create the response object
                    var responseObj = new IncomingMessage
                    {
                        UserName = incomingMessage.UserName,
                        UserId = userId,
                        Text = incomingMessage.Text,
                        Timestamp = incomingMessage.Timestamp,
                        Id = incomingMessage.Id
                    };

                    // Serialize and send the response
                    string jsonResponse = JsonSerializer.Serialize(incomingMessage);
                    byte[] response = Encoding.UTF8.GetBytes(jsonResponse);

                    await targetWebSocket.SendAsync(new ArraySegment<byte>(response), WebSocketMessageType.Text, true, CancellationToken.None);
                    Console.WriteLine($"Message forwarded to {targetUserId}");
                }
                else
                {
                    Console.WriteLine($"Target user {targetUserId} not found");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}

class IncomingMessage
{
    public string Text { get; set; }
    public string UserId { get; set; } // Target user's ID
    public string UserName { get; set; }
    public DateTime Timestamp { get; set; }
    public string Id { get; set; }
}
