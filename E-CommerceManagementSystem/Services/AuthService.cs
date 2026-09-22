using Azure.Core;
using E_CommerceManagementSystem.Data;
using E_CommerceManagementSystem.Dto;
using E_CommerceManagementSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace E_CommerceManagementSystem.Services
{
    public class AuthService(AppDbContext dbContext,IRefreshTokenService refreshTokenService,
        ITokenService tokenService,IAuditLogService auditLogService) : IAuthService
    {
        

        public async Task<RegisterResponseDto?> Register(RegisterRequestDto request)
        {

            var userexist = await dbContext.Users.AnyAsync(x => x.UserName == request.UserName || x.Email == request.Email);
            if (userexist)
            {
                return null;
            }
            var user = new Users();
            var passwordhashed = new PasswordHasher<Users>().HashPassword(user, request.Password);
            user.UserName = request.UserName;
            user.PasswordHashed = passwordhashed;
            user.Email = request.Email;
            user.CreatedAt = DateTime.Now;
            
            
            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();
            var cart = new Cart
            {
                UserId = user.Id
            };

            var wishlist = new Wishlist
            {
                UserId = user.Id
            };

            dbContext.Carts.Add(cart);
            dbContext.Wishlists.Add(wishlist);

            await dbContext.SaveChangesAsync();
            var auth = new AuditLogRequest();
            auth.UserId = user.Id;
            await auditLogService.CreateAsync(auth, AuditAction.Register);
            return new RegisterResponseDto
            {
                UserName = user.UserName,
                Email = user.Email,
            };
        }
        
        public async Task<TokenResponseDto?> Login(LoginRequestDto request)
        {
            var user = await dbContext.Users.FirstOrDefaultAsync(x => x.UserName == request.UsernameOrEmail || x.Email == request.UsernameOrEmail);
            if (user == null) return null;
            var result = new PasswordHasher<Users>().VerifyHashedPassword(user, user.PasswordHashed, request.Password);
            if (result == PasswordVerificationResult.Failed) return null;
            var auth = new AuditLogRequest();
            auth.UserId = user.Id;
            await auditLogService.CreateAsync(auth, AuditAction.Login);

            return new TokenResponseDto 
            {
                AccessToken =  tokenService.CreateToken(user),
                RefreshToken = await refreshTokenService.CreateRefreshToken(user),
            };

        }
        public async Task<UserProfileResponseDto?> GetProfile(int userId)
        {
            return await dbContext.Users
                .Where(x => x.Id == userId)
                .Select(x => new UserProfileResponseDto
                {
                    Id = x.Id,
                    UserName = x.UserName,
                    Email = x.Email
                })
                .FirstOrDefaultAsync();
        }
        public async Task<bool> Logout(int userId, LogoutRequestDto request)
        {
            var hashtoken = refreshTokenService.HashToken(request.RefreshToken);
            var token = await dbContext.RefreshTokens.FirstOrDefaultAsync(x => x.HashToken == hashtoken &&
            x.RevokedAt == null);
            if (token is null) return false;
            token.RevokedAt = DateTime.UtcNow;
            var auth = new AuditLogRequest();
            auth.UserId = userId;
            await auditLogService.CreateAsync(auth, AuditAction.Logout);
            await dbContext.SaveChangesAsync();
            return true;
        }
    }
}
