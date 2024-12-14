using WebSocketServer.Application.ViewModels;
using WebSocketServer.Domain.Entities;
using WebSocketServer.Domain.Interfaces;
using WebSocketServer.Domain.RequestModels;

namespace WebSocketServer.Application.UseCases
{
    public class UserManagementUseCase
    {
        private readonly IFirebaseUserService _firebaseUserService;

        public UserManagementUseCase(IFirebaseUserService firebaseUserService)
        {
            _firebaseUserService = firebaseUserService;
        }

        public Task<User> RegisterUserAsync(User user) => _firebaseUserService.RegisterUserAsync(user);

        public Task<User> LoginUserAsync(LoginRequest request) => _firebaseUserService.LoginUserAsync(request);

        public Task<IEnumerable<User>> GetAllUsersAsync() => _firebaseUserService.GetAllUsersAsync();

        public Task<User> EditUserAsync(string uid, User updatedUser) => _firebaseUserService.EditUserAsync(uid, updatedUser);

        public Task DeleteUserAsync(string uid) => _firebaseUserService.DeleteUserAsync(uid);
    }
}
