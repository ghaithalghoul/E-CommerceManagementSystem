using E_CommerceManagementSystem.Models;

namespace E_CommerceManagementSystem.Dto.Order
{
    public class OrderFilterRequest
    {
        public OrderStatus? OrderStatus { get; set; }
        public int? UserId { get; set; }
        public int page { get; set; } = 1;
        public int pagesize { get; set; } = 1;
    }
}
