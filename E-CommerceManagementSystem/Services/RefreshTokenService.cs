using Azure.Core;
using E_CommerceManagementSystem.Data;
using E_CommerceManagementSystem.Dto;
using E_CommerceManagementSystem.Migrations;
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
            var refreshToken = tokenService.GenerateRefreshToken();
            var refreshTokenEntity = new RefreshToken
            {
                HashToken = HashToken(refreshToken),
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                UserId = user.Id
            };

            dbContext.RefreshTokens.Add(refreshTokenEntity);
            await dbContext.SaveChangesAsync();
            return refreshToken;
        }

        public string HashToken(string token)
        {
            using var sha256 = SHA256.Create();

            var bytes = Encoding.UTF8.GetBytes(token);

            var hash = sha256.ComputeHash(bytes);

            return Convert.ToBase64String(hash);
        }

        public async Task<TokenResponseDto?> RefreshTokenAsync(RefreshTokenRequestDto request)
        {
            var refreshTokenEntity = await ValidateRefreshToken(
                request.UserId,
                request.RefreshToken
            );

            if (refreshTokenEntity is null)
                return null;


            var user = refreshTokenEntity.User;


            // Revoke old refresh token
            refreshTokenEntity.RevokedAt = DateTime.UtcNow;


            var newRefreshToken = await CreateRefreshToken(user);


            return new TokenResponseDto
            {
                AccessToken = tokenService.CreateToken(user),
                RefreshToken = newRefreshToken
            };


        }

        public async Task<RefreshToken?> ValidateRefreshToken(int userId, string token)
        {
            var hashToken = HashToken(token);


            var refreshToken = await dbContext.RefreshTokens
                .Include(x => x.User)
                .FirstOrDefaultAsync(x =>
                    x.UserId == userId &&
                    x.HashToken == hashToken);


            if (refreshToken is null)
                return null;


            if (refreshToken.ExpiresAt <= DateTime.UtcNow)
                return null;


            if (refreshToken.RevokedAt != null)
                return null;


            return refreshToken;
        }
    }
}
