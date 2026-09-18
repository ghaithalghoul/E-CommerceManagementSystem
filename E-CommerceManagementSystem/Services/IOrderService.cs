using E_CommerceManagementSystem.Dto.Order;
using E_CommerceManagementSystem.Models;

namespace E_CommerceManagementSystem.Services
{
    public interface IOrderService
    {
        Task<Order?> CreateOrder(int userId);
        Task<List<Order>> GetAllOrders(int userId);
        Task<Order?> GetOrder(int userId,int OrderId);
        Task<Order?> CancelOrder(int userId,int OrderId);
        Task<Order?> UpdateOrderStatus(int adminId, UpdateOrderRequest request);
        
    }
}
