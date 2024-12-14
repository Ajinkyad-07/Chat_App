using Microsoft.AspNetCore.Mvc;
using WebSocketServer.Application.UseCases;
using WebSocketServer.Domain.Entities;
using WebSocketServer.Domain.RequestModels;

namespace WebSocketServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserManagementUseCase _userManagementUseCase;

        public UserController(UserManagementUseCase userManagementUseCase)
        {
            _userManagementUseCase = userManagementUseCase;
        }

        [HttpPost("register")]
        public async Task<User> Register([FromBody] User user)
        {
            var registerdUser = await _userManagementUseCase.RegisterUserAsync(user);
            return registerdUser;
        }

        [HttpPost("login")]
        public async Task<User> Login([FromBody] LoginRequest request)
        {
            var response = await _userManagementUseCase.LoginUserAsync(request);
            return response;
        }

        [HttpGet("all")]
        public async Task<List<User>> GetAllUsers()
        {
            var users = await _userManagementUseCase.GetAllUsersAsync();
            return users.ToList();
        }

        [HttpPut("edit/{uid}")]
        public async Task<User> Edit(string uid, [FromBody] User user)
        {
            var updatedUser = await _userManagementUseCase.EditUserAsync(uid, user);
            return updatedUser;
        }

        [HttpDelete("delete/{uid}")]
        public async Task<IActionResult> Delete(string uid)
        {
            await _userManagementUseCase.DeleteUserAsync(uid);
            return Ok(new { Message = "User deleted successfully" });
        }
    }
}
