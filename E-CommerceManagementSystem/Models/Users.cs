namespace E_CommerceManagementSystem.Models
{
    public class Users
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHashed { get; set; } = string.Empty;
        public string Role { get; set; } = "Customer";
        public DateTime CreatedAt { get; set; }
        public string? RefreshToken {  get; set; }
        public DateTime? RefreshTokenExpireTime { get; set; }
        public Cart? Cart { get; set; }
        public ICollection<Order> orders { get; set; } = new List<Order>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public Wishlist? Wishlist { get; set; }
    }
}
