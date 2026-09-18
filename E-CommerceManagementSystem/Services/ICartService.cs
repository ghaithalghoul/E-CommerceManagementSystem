using E_CommerceManagementSystem.Dto;
using E_CommerceManagementSystem.Dto.Cart;
using E_CommerceManagementSystem.Models;

namespace E_CommerceManagementSystem.Services
{
    public interface ICartService
    {
        Task<CartResponse?> GetCart(int userId,int id);
        Task<CartItemResponseDto?> AddCartIteme(int userId,AddCartItemRequest request);
        Task<CartItemResponseDto?> UpdateCartItme(int userId, UpdateCartItmeRequest request, int cartitemId);
        Task<CartItemResponseDto?> DeleteCartItem(int id ,int userId);
        Task<CartResponse?> DeleteCart(int userId);
    }
}
