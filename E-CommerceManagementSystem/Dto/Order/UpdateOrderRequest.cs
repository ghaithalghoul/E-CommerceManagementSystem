using E_CommerceManagementSystem.Models;

namespace E_CommerceManagementSystem.Dto.Order
{
    public class UpdateOrderRequest
    {
        public required int OrderId { get; set; }
        public required OrderStatus OrderStatus { get; set; }
    }
}
