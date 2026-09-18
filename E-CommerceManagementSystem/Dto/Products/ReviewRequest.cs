namespace E_CommerceManagementSystem.Dto.Products
{
    public class ReviewRequest
    {
        public required int ProductId { get; set; }
        public required int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
    }
}
