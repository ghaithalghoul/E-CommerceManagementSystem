namespace E_CommerceManagementSystem.Models
{
    public class AuditLog
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public Users User { get; set; } = null!;
        public AuditAction ActionType { get; set; }
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? IpAddress { get; set; } 
    }
}
