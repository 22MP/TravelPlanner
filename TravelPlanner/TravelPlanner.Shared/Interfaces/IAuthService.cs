using Microsoft.ServiceFabric.Services.Remoting;
using TravelPlanner.Shared.DTOs.Auth;


namespace TravelPlanner.Shared.Interfaces
{
    public interface IAuthService : IService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request);
        Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<bool> DeleteUserAsync(int userId);
    }
}