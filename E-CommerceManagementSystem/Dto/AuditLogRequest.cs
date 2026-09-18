using E_CommerceManagementSystem.Models;

namespace E_CommerceManagementSystem.Dto
{
    public class AuditLogRequest
    {
        public int UserId { get; set; }
        
        
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        
        public string? IpAddress { get; set; }
    }
}
