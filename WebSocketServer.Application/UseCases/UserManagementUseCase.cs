using WebSocketServer.Domain.Entities;
using WebSocketServer.Domain.Interfaces;

namespace WebSocketServer.Application.UseCases
{
    public class UserManagementUseCase
    {
        private readonly IFirebaseUserService _firebaseUserService;

        public UserManagementUseCase(IFirebaseUserService firebaseUserService)
        {
            _firebaseUserService = firebaseUserService;
        }

        public Task<string> RegisterUserAsync(User user) => _firebaseUserService.RegisterUserAsync(user);

        public Task<string> LoginUserAsync(string email, string password) => _firebaseUserService.LoginUserAsync(email, password);

        public Task<IEnumerable<User>> GetAllUsersAsync() => _firebaseUserService.GetAllUsersAsync();

        public Task<User> EditUserAsync(string uid, User updatedUser) => _firebaseUserService.EditUserAsync(uid, updatedUser);

        public Task DeleteUserAsync(string uid) => _firebaseUserService.DeleteUserAsync(uid);
    }
}
