using Microsoft.AspNetCore.Mvc;
using WebSocketServer.Application.UseCases;
using WebSocketServer.Domain.Entities;

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
        public async Task<IActionResult> Register([FromBody] User user)
        {
            var uid = await _userManagementUseCase.RegisterUserAsync(user);
            return Ok(new { UserId = uid });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(string email, string password)
        {
            var uid = await _userManagementUseCase.LoginUserAsync(email, password);
            return Ok(new { UserId = uid });
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userManagementUseCase.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpPut("edit/{uid}")]
        public async Task<IActionResult> Edit(string uid, [FromBody] User user)
        {
            var updatedUser = await _userManagementUseCase.EditUserAsync(uid, user);
            return Ok(updatedUser);
        }

        [HttpDelete("delete/{uid}")]
        public async Task<IActionResult> Delete(string uid)
        {
            await _userManagementUseCase.DeleteUserAsync(uid);
            return Ok(new { Message = "User deleted successfully" });
        }
    }

}
