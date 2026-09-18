using E_CommerceManagementSystem.Models;

namespace E_CommerceManagementSystem.Services
{
    public interface ITokenService
    {
        string CreateToken(Users user);

        string GenerateRefreshToken();
    }
}
