using E_CommerceManagementSystem.Dto;
using E_CommerceManagementSystem.Migrations;
using E_CommerceManagementSystem.Models;

namespace E_CommerceManagementSystem.Services
{
    public interface IWishlistService
    {
        Task<List<WishlistItemResponseDto>> GetWishlistAsync(int userId);
        Task<WishlistItemResponseDto?> AddWishlistItemAsync(int userId, int productId);
        Task<WishlistItemResponseDto?> DeleteWishlistItemAsync(int userId, int productId);
    }
}
