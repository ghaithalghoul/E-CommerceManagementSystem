using E_CommerceManagementSystem.Dto;
using E_CommerceManagementSystem.Models;

namespace E_CommerceManagementSystem.Services
{
    public interface IAuditLogService
    {
        Task<AuditLog> CreateAsync(
         AuditLogRequest request,
         AuditAction action);
    }
}
