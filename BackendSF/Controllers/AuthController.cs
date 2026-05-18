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

        /// <summary>
        /// Registracija novog korisnika
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _authService.RegisterAsync(request);

            if (string.IsNullOrEmpty(response.Token))
                return BadRequest(new { message = "Korisnik sa ovim emailom vec postoji." });

            return Ok(response);
        }

        /// <summary>
        /// Logovanje korisnika
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _authService.LoginAsync(request);

            if (string.IsNullOrEmpty(response.Token))
                return Unauthorized(new { message = "Pogresan email ili lozinka." });

            return Ok(response);
        }

        /// <summary>
        /// Dohvati sve korisnika - samo za admina
        /// </summary>
        [HttpGet("users")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _authService.GetAllUsersAsync();
            return Ok(users);
        }

        /// <summary>
        /// Brisanje korisnika - samo za admina
        /// </summary>
        [HttpDelete("users/{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await _authService.DeleteUserAsync(id);

            if (!result)
                return NotFound(new { message = "Korisnik nije pronadjen." });

            return NoContent();
        }
    }
}