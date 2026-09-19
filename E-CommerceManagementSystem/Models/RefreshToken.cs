namespace E_CommerceManagementSystem.Models
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string HashToken { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime? RevokedAt { get; set; }
        public int UserId { get; set; }
        public Users User { get; set; } = null!;
    }
}
