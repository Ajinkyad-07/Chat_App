using WebSocketServer.Domain.Entities;

namespace WebSocketServer.Domain.Interfaces
{
    public interface IFirebaseUserService
    {
        Task<string> RegisterUserAsync(User user);
        Task<string> LoginUserAsync(string email, string password);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User> EditUserAsync(string uid, User updatedUser);
        Task DeleteUserAsync(string uid);
    }
}
