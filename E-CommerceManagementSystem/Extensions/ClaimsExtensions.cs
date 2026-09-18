using System.Security.Claims;

namespace E_CommerceManagementSystem.Extensions
{
    public static class ClaimsExtensions
    {
        public static int GetUserId(this ClaimsPrincipal user) { 
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier); 
            if (string.IsNullOrEmpty(userId)) throw new UnauthorizedAccessException("User ID not found");
            return int.Parse(userId); 
        }
    }
}
