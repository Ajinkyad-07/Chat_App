using System.Net.WebSockets;
using WebSocketServer.Domain.Entities;

namespace WebSocketServer.Domain.Interfaces
{
    public interface IWebSocketManager
    {
        Task HandleConnectionAsync(string userId, WebSocket webSocket);
        Task SendMessageAsync(string targetUserId, IncomingMessage message);
    }
}
