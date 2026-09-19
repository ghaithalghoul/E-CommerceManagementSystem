using E_CommerceManagementSystem.Models;

namespace E_CommerceManagementSystem.Dto.Order
{
    public class OrderResponse
    {
        public int Id { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime CreatedAt { get; set; }
        public int UserId { get; set; }

        public List<OrderItemResponseDto> Items { get; set; } = new();
    }
}
