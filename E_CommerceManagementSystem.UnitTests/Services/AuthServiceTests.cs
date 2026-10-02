using Azure.Core;
using E_CommerceManagementSystem.Dto;
using E_CommerceManagementSystem.Models;
using E_CommerceManagementSystem.Services;
using E_CommerceManagementSystem.UnitTests.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_CommerceManagementSystem.UnitTests.Services
{
    public class AuthServiceTests
    {
        [Fact]
        public async Task Register_ValidRequest_ReturnsUserAndCreatesRelatedEntities()
        {
            await using var dbContext = TestDbContextFactory.Create();

            var request = new RegisterRequestDto
            {
                UserName = "demo1",
                Email = "demo1@gmail.com",
                Password = "12345678"
            };

            var mockRefreshTokenService = new Mock<IRefreshTokenService>();
            var mockTokenService = new Mock<ITokenService>();
            var mockAuditLogService = new Mock<IAuditLogService>();

            var service = new AuthService(
                dbContext,
                mockRefreshTokenService.Object,
                mockTokenService.Object,
                mockAuditLogService.Object);

            var result = await service.Register(request);

            var user = await dbContext.Users
                .FirstOrDefaultAsync(x => x.UserName == "demo1");

            Assert.NotNull(result);
            Assert.Equal("demo1", result.UserName);
            Assert.Equal("demo1@gmail.com", result.Email);

            Assert.NotNull(user);
            Assert.NotEqual("12345678", user.PasswordHashed);

            var cart = await dbContext.Carts
                .FirstOrDefaultAsync(x => x.UserId == user.Id);

            var wishlist = await dbContext.Wishlists
                .FirstOrDefaultAsync(x => x.UserId == user.Id);

            Assert.NotNull(cart);
            Assert.NotNull(wishlist);
            var passwordHasher = new PasswordHasher<Users>();

            var verificationResult = passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHashed,
                request.Password);

            Assert.Equal(
                PasswordVerificationResult.Success,
                verificationResult);

            mockAuditLogService.Verify(
                x => x.CreateAsync(
                    It.Is<AuditLogRequest>(a => a.UserId == user.Id),
                    AuditAction.Register),
                Times.Once);
        }



        [Fact]
        public async Task Resiter_UerNameAlreadyExist_ReturnNull()
        {
            await using var dbContext = TestDbContextFactory.Create();
            var user = new Users { Id = 1 ,UserName="demo1",Email="demo2@gmail.com"};
            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();

            var request = new RegisterRequestDto
            {
                UserName = "demo1",
                Email = "demo1@gmail.com",
                Password = "12345678"
            };
            var mockRefreshTokenService = new Mock<IRefreshTokenService>();
            var mockTokenService = new Mock<ITokenService>();
            var mockAuditLogService = new Mock<IAuditLogService>();

            var service = new AuthService(
                dbContext,
                mockRefreshTokenService.Object,
                mockTokenService.Object,
                mockAuditLogService.Object);
            var result = await service.Register(request);
            Assert.Null(result);
            var usersCount = await dbContext.Users.CountAsync();

            Assert.Equal(1, usersCount);
        }

        [Fact]
        public async Task Resiter_EmailAlreadyExist_ReturnNull()
        {
            await using var dbContext = TestDbContextFactory.Create();
            var user = new Users { Id = 1, UserName = "demo2", Email = "demo1@gmail.com" };
            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();

            var request = new RegisterRequestDto
            {
                UserName = "demo1",
                Email = "demo1@gmail.com",
                Password = "12345678"
            };
            var mockRefreshTokenService = new Mock<IRefreshTokenService>();
            var mockTokenService = new Mock<ITokenService>();
            var mockAuditLogService = new Mock<IAuditLogService>();

            var service = new AuthService(
                dbContext,
                mockRefreshTokenService.Object,
                mockTokenService.Object,
                mockAuditLogService.Object);
            var result = await service.Register(request);
            Assert.Null(result);
            var usersCount = await dbContext.Users.CountAsync();

            Assert.Equal(1, usersCount);
        }
        [Fact]
         public async Task login_ValidRequestUsername_ReturnToken()
        {
            await using var dbContext = TestDbContextFactory.Create();
            var testuser = new Users
            {
                UserName = "ghaith",
                Email = "ghaith@gmail.com"
            };
            var password = "12345678";
            var passwordhashed = new PasswordHasher<Users>().HashPassword(testuser, password);
            var user = new Users { Id = 1, UserName = testuser.UserName, Email = testuser.Email,PasswordHashed=passwordhashed};
            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();
            var mockRefreshTokenService = new Mock<IRefreshTokenService>();
            var mockTokenService = new Mock<ITokenService>();
            var mockAuditLogService = new Mock<IAuditLogService>();

            var service = new AuthService(
                dbContext,
                mockRefreshTokenService.Object,
                mockTokenService.Object,
                mockAuditLogService.Object);

            mockTokenService
                .Setup(x => x.CreateToken(It.IsAny<Users>()))
                .Returns("access-token");

            mockRefreshTokenService
                .Setup(x => x.CreateRefreshToken(It.IsAny<Users>()))
                .ReturnsAsync("refresh-token");

            var request = new LoginRequestDto { UsernameOrEmail = "ghaith", Password = "12345678" };
            var result = await service.Login(request);
            Assert.NotNull(result);
            Assert.Equal("access-token", result.AccessToken);
            Assert.Equal("refresh-token", result.RefreshToken);
            mockTokenService.Verify(
            x => x.CreateToken(It.IsAny<Users>()),
            Times.Once);
            mockRefreshTokenService.Verify(
            x => x.CreateRefreshToken(It.IsAny<Users>()),
            Times.Once);
            mockAuditLogService.Verify(
            x => x.CreateAsync(
                It.Is<AuditLogRequest>(a => a.UserId == 1),
                AuditAction.Login),
            Times.Once);

        }
        [Fact]
        public async Task login_ValidRequestEmail_ReturnToken()
        {
            await using var dbContext = TestDbContextFactory.Create();
            var testuser = new Users
            {
                UserName = "ghaith",
                Email = "ghaith@gmail.com"
            };
            var password = "12345678";
            var passwordhashed = new PasswordHasher<Users>().HashPassword(testuser, password);
            var user = new Users { Id = 1, UserName = testuser.UserName, Email = testuser.Email, PasswordHashed = passwordhashed };
            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();
            var mockRefreshTokenService = new Mock<IRefreshTokenService>();
            var mockTokenService = new Mock<ITokenService>();
            var mockAuditLogService = new Mock<IAuditLogService>();

            var service = new AuthService(
                dbContext,
                mockRefreshTokenService.Object,
                mockTokenService.Object,
                mockAuditLogService.Object);

            mockTokenService
                .Setup(x => x.CreateToken(It.IsAny<Users>()))
                .Returns("access-token");

            mockRefreshTokenService
                .Setup(x => x.CreateRefreshToken(It.IsAny<Users>()))
                .ReturnsAsync("refresh-token");

            var request = new LoginRequestDto { UsernameOrEmail = "ghaith@gmail.com", Password = "12345678" };
            var result = await service.Login(request);
            Assert.NotNull(result);
            Assert.Equal("access-token", result.AccessToken);
            Assert.Equal("refresh-token", result.RefreshToken);
            mockTokenService.Verify(
            x => x.CreateToken(It.IsAny<Users>()),
            Times.Once);
            mockRefreshTokenService.Verify(
            x => x.CreateRefreshToken(It.IsAny<Users>()),
            Times.Once);
            mockAuditLogService.Verify(
            x => x.CreateAsync(
                It.Is<AuditLogRequest>(a => a.UserId == 1),
                AuditAction.Login),
            Times.Once);

        }
        [Fact]
        public async Task Login_WrongUsernameOrEmail_ReturnNull()
        {
            await using var dbContext = TestDbContextFactory.Create();
            var testuser = new Users
            {
                UserName = "ghaith",
                Email = "ghaith@gmail.com"
            };
            var password = "12345678";
            var passwordhashed = new PasswordHasher<Users>().HashPassword(testuser, password);
            var user = new Users { Id = 1, UserName = testuser.UserName, Email = testuser.Email, PasswordHashed = passwordhashed };
            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();
            var mockRefreshTokenService = new Mock<IRefreshTokenService>();
            var mockTokenService = new Mock<ITokenService>();
            var mockAuditLogService = new Mock<IAuditLogService>();

            var service = new AuthService(
                dbContext,
                mockRefreshTokenService.Object,
                mockTokenService.Object,
                mockAuditLogService.Object);
            var request = new LoginRequestDto { UsernameOrEmail = "wrongusernameoremail", Password = "12345678" };
            var result = await service.Login(request);
            Assert.Null(result);

        }
        [Fact]
        public async Task Login_WrongPassword_ReturnNull()
        {
            await using var dbContext = TestDbContextFactory.Create();

            var user = new Users
            {
                Id = 1,
                UserName = "ghaith",
                Email = "ghaith@gmail.com"
            };

            user.PasswordHashed =
                new PasswordHasher<Users>()
                    .HashPassword(user, "12345678");

            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();

            var mockRefreshTokenService = new Mock<IRefreshTokenService>();
            var mockTokenService = new Mock<ITokenService>();
            var mockAuditLogService = new Mock<IAuditLogService>();

            var service = new AuthService(
                dbContext,
                mockRefreshTokenService.Object,
                mockTokenService.Object,
                mockAuditLogService.Object);

            var request = new LoginRequestDto
            {
                UsernameOrEmail = "ghaith",
                Password = "wrongpassword"
            };

            var result = await service.Login(request);

            Assert.Null(result);
        }
    }
}
