using Microsoft.Extensions.DependencyInjection;
using WebSocketServer.Application.UseCases;
using WebSocketServer.Domain.Interfaces;
using WebSocketServer.Infrastructure.Services;

namespace WebSocketServer.Infrastructure.Extensions
{
    public static class Extensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddSingleton<IWebSocketManager, WebSocketManager>();
            services.AddScoped<IFirebaseUserService, FirebaseUserService>();
            services.AddScoped<UserManagementUseCase>();
            return services;
        }
    }
}
