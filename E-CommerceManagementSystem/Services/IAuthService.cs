using E_CommerceManagementSystem.Dto;

namespace E_CommerceManagementSystem.Services
{
    public interface IAuthService
    {
        Task<RegisterResponseDto?> Register(RegisterRequestDto request);
        Task<TokenResponseDto?> Login(LoginRequestDto request);
        Task<bool> Logout(int userId);
    }
}
