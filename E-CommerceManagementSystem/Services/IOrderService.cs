using E_CommerceManagementSystem.Dto.Order;
using E_CommerceManagementSystem.Models;

namespace E_CommerceManagementSystem.Services
{
    public interface IOrderService
    {
        Task<OrderResponse?> CreateOrder(int userId);
        Task<List<OrderResponse>> GetAllOrders(int userId);
        Task<OrderResponse?> GetOrder(int userId,int OrderId);
        Task<OrderResponse?> CancelOrder(int userId,int OrderId);
        Task<OrderResponse?> UpdateOrderStatus(int adminId, UpdateOrderRequest request);
        
    }
}
