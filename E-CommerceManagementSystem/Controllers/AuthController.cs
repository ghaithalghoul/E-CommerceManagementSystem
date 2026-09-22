using E_CommerceManagementSystem.Data;
using E_CommerceManagementSystem.Dto;
using E_CommerceManagementSystem.Extensions;
using E_CommerceManagementSystem.Models;
using E_CommerceManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_CommerceManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService , IRefreshTokenService refreshtokenService) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<ActionResult<RegisterResponseDto>> Register(RegisterRequestDto requst)
        {
            var result = await authService.Register(requst);
            if (result is null) return BadRequest("Somthing went wrong");
            return StatusCode(201, result);
        }
        [HttpPost("login")]
        public async Task<ActionResult<TokenResponseDto>> Login(LoginRequestDto requst) 
        { 
            var result = await authService.Login(requst);
            if (result is null) return BadRequest("Username or email or password is wrong");
            return Ok(result);
        }
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshTokenAsync(RefreshTokenRequestDto request)
        {
            var result = await refreshtokenService.RefreshTokenAsync(request);
            if (result is null) return BadRequest("somthing went wrong");
            return Ok(result);
        }
        [Authorize]
        [HttpGet("me")]
        public async Task<ActionResult<UserProfileResponseDto>> GetProfile()
        {
            var userId = User.GetUserId();

            var user = await authService.GetProfile(userId);

            if (user is null)
                return NotFound();

            return Ok(user);
        }
        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout(LogoutRequestDto request)
        {
            var userid = User.GetUserId();
            var result = await authService.Logout(userid, request);
            if (result == false) return BadRequest("Somthing went wrong");
            return Ok(new {
                message = "Logged out successfully" 
            });
        }
    }
}
