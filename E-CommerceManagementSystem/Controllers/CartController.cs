using E_CommerceManagementSystem.Dto;
using E_CommerceManagementSystem.Dto.Cart;
using E_CommerceManagementSystem.Extensions;
using E_CommerceManagementSystem.Models;
using E_CommerceManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_CommerceManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController(ICartService cartService) : ControllerBase
    {
        [Authorize]
        [HttpGet("{cartId:int}")]
        public  async Task<ActionResult<CartResponse?>> GetCart( int id)
        {
            var userId = User.GetUserId();
            var result =await cartService.GetCart(userId, id);
            if (result == null) return NotFound();
            return Ok(result);
        }
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<CartItemResponseDto?>> AddCartIteme( AddCartItemRequest request)
        {
            var userId = User.GetUserId();
            var result = await cartService.AddCartIteme(userId, request);
            if(result == null) return BadRequest();
            return StatusCode(201,result);
        }
        [Authorize]
        [HttpPut]
        public async Task<ActionResult<CartItemResponseDto?>> UpdateCartIteme(UpdateCartItmeRequest request, int cartitemId)
        {
            var userId = User.GetUserId();
            var result = await cartService.UpdateCartItme(userId, request, cartitemId);
            if (result == null) return BadRequest();
            return StatusCode(201, result);
        }
        [Authorize]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteCartItem(int id)
        {
            var userId= User.GetUserId();
            var result  = await cartService.DeleteCartItem(userId, id);
            if( result == null) return NotFound();
            return Ok(result);
        }
        [Authorize]
        [HttpDelete("delete-cart")]
        public async Task<IActionResult> DeleteCart()
        {
            var userId = User.GetUserId();
            var result = await cartService.DeleteCart(userId);
            if (result == null) return NotFound();
            return Ok(result);
        }
    }
}
