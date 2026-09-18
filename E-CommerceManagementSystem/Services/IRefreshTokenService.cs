using E_CommerceManagementSystem.Dto;
using E_CommerceManagementSystem.Migrations;
using E_CommerceManagementSystem.Models;

namespace E_CommerceManagementSystem.Services
{
    public interface IRefreshTokenService
    {
        Task<string> CreateRefreshToken(Users user);

        Task<Users?> ValidateRefreshToken(
            int userId,
            string token);
        Task<string?> RefreshTokenAsync(RefreshTokenRequestDto requst);
        string HashToken(string token);
    }
}
