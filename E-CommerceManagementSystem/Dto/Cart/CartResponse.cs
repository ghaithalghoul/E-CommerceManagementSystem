using E_CommerceManagementSystem.Models;

namespace E_CommerceManagementSystem.Dto.Cart
{
    public class CartResponse
    {
        public int CartId { get; set; }
        public List<CartItemResponseDto> Items { get; set; } = new();
        public decimal Total { get; set; }
    }
}
