namespace E_CommerceManagementSystem.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public Users User { get; set; } = null!;
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
