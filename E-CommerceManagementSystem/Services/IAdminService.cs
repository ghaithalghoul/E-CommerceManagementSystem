using E_CommerceManagementSystem.Dto;
using E_CommerceManagementSystem.Dto.Admin;
using E_CommerceManagementSystem.Dto.Order;

namespace E_CommerceManagementSystem.Services
{
    public interface IAdminService
    {
        Task<PaginatedResponse<UserResponseDto>> GetAllUsers(int page, int pagesize);
        Task<UserResponseDto?> GetUser(int userId);
        Task<ChangeUserRoleRequestDto?> ChangeUserRole(int userId, ChangeUserRoleRequest request);
        Task<UserResponseDto?> DeleteUser(int userId);
        Task<ReviewResponse?> DeleteReview(int reviewId);
        Task<PaginatedResponse<ReviewResponse>> GetReviews(ReviewFilterRequest request);
        Task<DashboardResponse> GetDashboard();
        Task<PaginatedResponse<OrderResponse>> GetAllOrder(OrderFilterRequest request);

    }
}
