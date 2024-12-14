using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using WebSocketServer.Domain.Entities;
using WebSocketServer.Domain.Interfaces;

namespace WebSocketServer.Infrastructure.Services
{
    public class WebSocketManager : IWebSocketManager
    {
        private readonly ConcurrentDictionary<string, WebSocket> _connections = new();

        public async Task HandleConnectionAsync(string userId, WebSocket webSocket)
        {
            _connections[userId] = webSocket;
            Console.WriteLine($"User connected: {userId}");

            var buffer = new byte[1024 * 4];
            while (webSocket.State == WebSocketState.Open)
            {
                try
                {
                    var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    if (result.CloseStatus.HasValue)
                    {
                        _connections.TryRemove(userId, out _);
                        await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
                        Console.WriteLine($"User disconnected: {userId}");
                        break;
                    }

                    string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    Console.WriteLine($"Message from {userId}: {message}");

                    var incomingMessage = JsonSerializer.Deserialize<IncomingMessage>(message);
                    if (incomingMessage != null)
                    {
                        await SendMessageAsync(incomingMessage.UserId, incomingMessage);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }

        public async Task SendMessageAsync(string targetUserId, IncomingMessage message)
        {
            if (_connections.TryGetValue(targetUserId, out var targetWebSocket) && targetWebSocket.State == WebSocketState.Open)
            {
                string jsonResponse = JsonSerializer.Serialize(message);
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
}
