using E_CommerceManagementSystem.Dto;
using E_CommerceManagementSystem.Dto.Admin;
using E_CommerceManagementSystem.Dto.Order;
using E_CommerceManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_CommerceManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController(IAdminService adminService) : ControllerBase
    {
        
        [HttpGet("users")]
        public async Task<ActionResult<PaginatedResponse<UserResponseDto>>> GetUsers(int page, int pageSize)
        {
            var result =await adminService.GetAllUsers(page, pageSize);
            return Ok(result);
        }
        
        [HttpGet("user/{id:int}")]
        public async Task<ActionResult<UserResponseDto?>> GetUser(int id)
        {
            var result = await adminService.GetUser(id);
            if (result is null) return NotFound();
            return Ok(result);
        }
        
        [HttpPut("update-user-role/{id:int}")]
        public async Task<ActionResult<ChangeUserRoleRequestDto?>> UpdateUserRole(int id, ChangeUserRoleRequest request)
        {
            var result = await adminService.ChangeUserRole(id, request);
            if (result is null) return NotFound();
            return Ok(result);
        }
        
        [HttpDelete("delete-user/{id:int}")]
        public async Task<ActionResult<UserResponseDto?>> DeleteUser(int id)
        {
            var result = await adminService.DeleteUser(id);
            if (result is null) return NotFound();
            return Ok(result);
        }
        
        [HttpDelete("delete-review/{id:int}")]
        public async Task<ActionResult<ReviewResponse?>> DeleteReview(int id)
        {
            var result = await adminService.DeleteReview(id);
            if (result is null) return NotFound();
            return Ok(result);
        }
        
        
        [HttpGet("reviews")]
        public async Task<ActionResult<PaginatedResponse<ReviewResponse>>> GetReviews(ReviewFilterRequest request)
        {
            var result = await adminService.GetReviews(request);
            return Ok(result);
        }
        [HttpGet("orders")]
        public async  Task<ActionResult<PaginatedResponse<OrderResponse>>> GetAllOrder(OrderFilterRequest request)
        {
            var result = await adminService.GetAllOrder(request);
            return Ok(result);
        }
        [HttpGet("dashboard")]
        public async Task<ActionResult<DashboardResponse>> GetDashboard()
        {
            return Ok(await adminService.GetDashboard());
        }
    }
}
