using E_CommerceManagementSystem.Dto.Order;
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
    public class OrderController(IOrderService orderService) : ControllerBase
    {
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<OrderResponse>> CreateOrder()
        {
            var userId = User.GetUserId();
            var order = await orderService.CreateOrder(userId);
            if (order == null) return BadRequest();
            return StatusCode(201, order);
        }
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<List<OrderResponse>>> GetAllOrders()
        {
            var userId = User.GetUserId();
            var orders = await orderService.GetAllOrders(userId);
            return Ok(orders);
        }
        [Authorize]
        [HttpGet("{orderId:int}")]
        public async Task<ActionResult<OrderResponse>> GetOrder(int orderId)
        {
            var userId = User.GetUserId();
            var order = await orderService.GetOrder(userId, orderId);
            if (order is null) return NotFound();
            return Ok(order);
        }
        [Authorize]
        [HttpPut("cancel/{orderId:int}")]
        public async Task<ActionResult<List<OrderResponse>>> CancelOrder(int orderId)
        {
            var userId = User.GetUserId();
            var order = await orderService.CancelOrder(userId, orderId);
            if (order is null) return NotFound();
            return Ok(order);
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("update-status")]
        public async Task<ActionResult<OrderResponse>> UpdateOrderStatus( UpdateOrderRequest request)
        {
            var adminId = User.GetUserId();
            var result = await orderService.UpdateOrderStatus(adminId,request);
            if(result is null) return NotFound();
            return Ok(result);

        }

    }
}
