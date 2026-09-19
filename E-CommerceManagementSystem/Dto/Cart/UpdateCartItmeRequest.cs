using System.ComponentModel.DataAnnotations;

namespace E_CommerceManagementSystem.Dto.Cart
{
    public class UpdateCartItmeRequest
    {
        [Range(1, 100)]
        public required int Quantity { get; set; }
    }
}
