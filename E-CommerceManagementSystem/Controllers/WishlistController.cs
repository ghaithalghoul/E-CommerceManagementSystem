using E_CommerceManagementSystem.Extensions;
using E_CommerceManagementSystem.Migrations;
using E_CommerceManagementSystem.Models;
using E_CommerceManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_CommerceManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WishlistController(IWishlistService wishlistService) : ControllerBase
    {
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<List<WishlistItem>>> GetWishList()
        {
            var userId = User.GetUserId();
            var result = await wishlistService.GetWishlistAsync(userId);
            return Ok(result);
        }
        [Authorize]
        [HttpPost("productId:int")]
        public async Task<ActionResult<List<WishlistItem>>> AddWishListItem(int productId)
        {
            var userId = User.GetUserId();
            var result = await wishlistService.AddWishlistItemAsync(userId,productId);
            if (result is null) return BadRequest();
            return Ok(result);
        }
        [Authorize]
        [HttpDelete("productId:int")]
        public async Task<ActionResult<List<WishlistItem>>> DeleteWishListItem(int productId)
        {
            var userId = User.GetUserId();
            var result = await wishlistService.DeleteWishlistItemAsync(userId, productId);
            if (result is null) return NotFound();
            return Ok(result);
        }
    }
}
