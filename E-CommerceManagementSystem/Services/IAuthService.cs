using E_CommerceManagementSystem.Dto;

namespace E_CommerceManagementSystem.Services
{
    public interface IAuthService
    {
        Task<RegisterResponseDto?> Register(RegisterRequestDto request);
        Task<TokenResponseDto?> Login(LoginRequestDto request);
        Task<UserProfileResponseDto?> GetProfile(int userId);
        Task<bool> Logout(int userId, LogoutRequestDto request);
    }
}
