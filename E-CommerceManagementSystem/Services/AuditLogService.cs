using E_CommerceManagementSystem.Data;
using E_CommerceManagementSystem.Dto;
using E_CommerceManagementSystem.Models;

namespace E_CommerceManagementSystem.Services
{
    public class AuditLogService(AppDbContext context) : IAuditLogService
    {
        public async Task<AuditLog> CreateAsync(
         AuditLogRequest request,
         AuditAction action)
        {
                var logger = new AuditLog
                {
                    UserId = request.UserId,
                    ActionType = action,
                    CreatedAt = DateTime.UtcNow,
                    OldValue =request.OldValue,
                    NewValue =request.NewValue,
                    IpAddress = request.IpAddress
                };

                context.AuditLogs.Add(logger);
                await context.SaveChangesAsync();

                return logger;
        }
    }
}
