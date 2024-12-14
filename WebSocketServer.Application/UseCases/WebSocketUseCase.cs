
using System.Net.WebSockets;
using WebSocketServer.Domain.Interfaces;
using WebSocketServer.Domain.Entities;

namespace WebSocketServer.Application.UseCases
{
    public class WebSocketUseCase
    {
        private readonly IWebSocketManager _webSocketManager;

        public WebSocketUseCase(IWebSocketManager webSocketManager)
        {
            _webSocketManager = webSocketManager;
        }

        public Task HandleConnectionAsync(string userId, WebSocket webSocket)
        {
            return _webSocketManager.HandleConnectionAsync(userId, webSocket);
        }

        public Task SendMessageAsync(string targetUserId, IncomingMessage message)
        {
            return _webSocketManager.SendMessageAsync(targetUserId, message);
        }
    }
}
