using WebSocketServer.Domain.Entities;
using WebSocketServer.Application.ViewModels;
using WebSocketServer.Domain.RequestModels;

namespace WebSocketServer.Domain.Interfaces
{
    public interface IFirebaseUserService
    {
        Task<User> RegisterUserAsync(User user);
        Task<User> LoginUserAsync(LoginRequest request);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User> EditUserAsync(string uid, User updatedUser);
        Task DeleteUserAsync(string uid);
    }
}
