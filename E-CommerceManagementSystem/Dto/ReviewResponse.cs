using E_CommerceManagementSystem.Models;

namespace E_CommerceManagementSystem.Dto
{
    public class ReviewResponse
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        
        public int ProductId { get; set; }
        
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
