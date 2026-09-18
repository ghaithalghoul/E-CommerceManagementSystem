namespace E_CommerceManagementSystem.Dto.Cart
{
    public class AddCartItemRequest
    {
        public required int ProductId { get; set; }
        public required int Quantity { get; set; }
    }
}
