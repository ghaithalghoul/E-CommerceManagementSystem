using E_CommerceManagementSystem.Data;
using E_CommerceManagementSystem.Dto;
using E_CommerceManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace E_CommerceManagementSystem.Services
{
    public class RefreshTokenService(AppDbContext dbContext, ITokenService tokenService) : IRefreshTokenService
    {
        public async Task<string> CreateRefreshToken(Users user)
        {
            var refreshtoken = tokenService.GenerateRefreshToken();
            user.RefreshToken = refreshtoken;
            user.RefreshTokenExpireTime = DateTime.UtcNow.AddDays(7);
            await dbContext.SaveChangesAsync();
            return refreshtoken;
        }

        public string HashToken(string token)
        {
            using var sha256 = SHA256.Create();

            var bytes = Encoding.UTF8.GetBytes(token);

            var hash = sha256.ComputeHash(bytes);

            return Convert.ToBase64String(hash);
        }

        public async Task<string?> RefreshTokenAsync(RefreshTokenRequestDto requst)
        {
            var result = await ValidateRefreshToken(requst.UserId, requst.RefreshToken);
            if (result == null) return null;
            var newRefreshtoken = tokenService.CreateToken(result);

            return newRefreshtoken;


        }

        public async Task<Users?> ValidateRefreshToken(int userId, string token)
        {
            var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user is null || user.RefreshToken != token || user.RefreshTokenExpireTime <= DateTime.UtcNow)
            {
                return null;
            }
            return user;
        }
    }
}
