namespace E_CommerceManagementSystem.Dto.Products
{
    public class UpdateReviewRequest
    {
        
        public int NewRating { get; set; }
        public string Comment { get; set; } = string.Empty;
    }
}
