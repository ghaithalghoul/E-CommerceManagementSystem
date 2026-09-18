using E_CommerceManagementSystem.Migrations;
using E_CommerceManagementSystem.Models;

namespace E_CommerceManagementSystem.Services
{
    public interface IWishlistService
    {
        Task<List<WishlistItem>> GetWishlistAsync(int userId);
        Task<WishlistItem?> AddWishlistItemAsync(int userId , int productId);
        Task<WishlistItem> DeleteWishlistItemAsync(int userId, int productId);
    }
}
