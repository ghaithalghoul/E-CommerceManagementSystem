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
        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder()
        {
            var userId = User.GetUserId();
            var order = await orderService.CreateOrder(userId);
            if (order == null) return BadRequest();
            return StatusCode(201, order);
        }
        [HttpGet]
        public async Task<ActionResult<List<Order>>> GetAllOrders()
        {
            var userId = User.GetUserId();
            var orders = await orderService.GetAllOrders(userId);
            return Ok(orders);
        }
        [HttpGet("{orderId:int}")]
        public async Task<ActionResult<Order>> GetOrder(int orderId)
        {
            var userId = User.GetUserId();
            var order = await orderService.GetOrder(userId, orderId);
            if (order is null) return NotFound();
            return Ok(order);
        }
        [HttpPut("cancel/{orderId:int}")]
        public async Task<ActionResult<List<Order>>> CancelOrder(int orderId)
        {
            var userId = User.GetUserId();
            var order = await orderService.CancelOrder(userId, orderId);
            if (order is null) return NotFound();
            return Ok(order);
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("update-status")]
        public async Task<ActionResult<Order>> UpdateOrderStatus( UpdateOrderRequest request)
        {
            var adminId = User.GetUserId();
            var result = await orderService.UpdateOrderStatus(adminId,request);
            if(result is null) return NotFound();
            return Ok(result);

        }

    }
}
