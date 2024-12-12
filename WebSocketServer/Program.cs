using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

// Enable WebSockets
app.UseWebSockets();

// Store active WebSocket connections
var connections = new ConcurrentDictionary<string, WebSocket>();

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
