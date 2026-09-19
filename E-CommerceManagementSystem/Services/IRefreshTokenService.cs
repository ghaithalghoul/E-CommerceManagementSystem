using E_CommerceManagementSystem.Dto;
using E_CommerceManagementSystem.Migrations;
using E_CommerceManagementSystem.Models;

namespace E_CommerceManagementSystem.Services
{
    public interface IRefreshTokenService
    {
        Task<string> CreateRefreshToken(Users user);

        Task<RefreshToken?> ValidateRefreshToken(
            int userId,
            string token);
        Task<TokenResponseDto?> RefreshTokenAsync(RefreshTokenRequestDto request);
        string HashToken(string token);
    }
}
