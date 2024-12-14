using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
using WebSocketServer.Application.UseCases;

namespace WebSocketServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WebSocketController : ControllerBase
    {
        private readonly WebSocketUseCase _webSocketUseCase;

        public WebSocketController(WebSocketUseCase webSocketUseCase)
        {
            _webSocketUseCase = webSocketUseCase;
        }

        [HttpGet("/ws")]
        public async Task HandleWebSocketAsync()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                string userId = HttpContext.Request.Query["userId"];
                if (string.IsNullOrEmpty(userId))
                {
                    HttpContext.Response.StatusCode = 400;
                    await HttpContext.Response.WriteAsync("User ID is required");
                    return;
                }

                WebSocket webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                await _webSocketUseCase.HandleConnectionAsync(userId, webSocket);
            }
            else
            {
                HttpContext.Response.StatusCode = 400;
                await HttpContext.Response.WriteAsync("WebSocket connection required");
            }
        }
    }
}
