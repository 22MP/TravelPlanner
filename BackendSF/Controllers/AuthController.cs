using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using TravelPlanner.Shared.DTOs.Auth;
using TravelPlanner.Shared.Interfaces;

namespace BackendSF.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController()
        {
            _authService = ServiceProxy.Create<IAuthService>(
                new Uri("fabric:/TravelPlanner/AuthService"));
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            var response = await _authService.RegisterAsync(request);
            if (string.IsNullOrEmpty(response.Token))
                return BadRequest(new { message = "Email already exists." });
            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var response = await _authService.LoginAsync(request);
            if (string.IsNullOrEmpty(response.Token))
                return Unauthorized(new { message = "Invalid email or password." });
            return Ok(response);
        }

        [HttpGet("users")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _authService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpDelete("users/{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await _authService.DeleteUserAsync(id);
            if (!result)
                return NotFound(new { message = "User not found." });
            return NoContent();
        }
    }
}