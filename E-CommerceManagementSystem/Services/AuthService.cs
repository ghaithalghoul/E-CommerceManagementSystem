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
            var user = await dbContext.Users.FirstOrDefaultAsync(x => x.UserName == request.UserName || x.Email == request.Email);
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

        public async Task<bool> Logout(int userId)
        {
            var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Id ==  userId);
            if (user is null) return false;
            user.RefreshToken = null;
            user.RefreshTokenExpireTime = null;
            var auth = new AuditLogRequest();
            auth.UserId = user.Id;
            await auditLogService.CreateAsync(auth, AuditAction.Logout);
            await dbContext.SaveChangesAsync();
            return true;
        }
    }
}
