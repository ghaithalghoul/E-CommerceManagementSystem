using E_CommerceManagementSystem.Data;
using E_CommerceManagementSystem.Dto;

using E_CommerceManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace E_CommerceManagementSystem.Services
{
    public class WishlistService(AppDbContext context) : IWishlistService
    {
        public async Task<WishlistItemResponseDto?> AddWishlistItemAsync(
            int userId,
            int productId)
        {
            var wishlist = await context.Wishlists
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (wishlist is null)
                return null;

            var product = await context.Products
                .FirstOrDefaultAsync(x => x.ProductId == productId);

            if (product is null)
                return null;

            var alreadyExists = await context.WishlistItems
                .AnyAsync(x =>
                    x.WishlistId == wishlist.Id &&
                    x.ProductId == productId);

            if (alreadyExists)
                return null;

            var wishlistItem = new WishlistItem
            {
                WishlistId = wishlist.Id,
                ProductId = productId
            };

            context.WishlistItems.Add(wishlistItem);

            await context.SaveChangesAsync();

            return new WishlistItemResponseDto
            {
                Id = wishlistItem.Id,
                ProductId = product.ProductId,
                ProductName = product.Name,
                Price = product.Price,
                ImageUrl = product.ImageUrl
            };
        }

        public async Task<WishlistItemResponseDto?> DeleteWishlistItemAsync(
            int userId,
            int productId)
        {
            var wishlist = await context.Wishlists
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (wishlist is null)
                return null;

            var wishlistItem = await context.WishlistItems
                .Include(x => x.Product)
                .FirstOrDefaultAsync(x =>
                    x.ProductId == productId &&
                    x.WishlistId == wishlist.Id);

            if (wishlistItem is null)
                return null;

            var response = new WishlistItemResponseDto
            {
                Id = wishlistItem.Id,
                ProductId = wishlistItem.ProductId,
                ProductName = wishlistItem.Product.Name,
                Price = wishlistItem.Product.Price,
                ImageUrl = wishlistItem.Product.ImageUrl
            };

            context.WishlistItems.Remove(wishlistItem);

            await context.SaveChangesAsync();

            return response;
        }

        public async Task<List<WishlistItemResponseDto>> GetWishlistAsync(
            int userId)
        {
            var wishlist = await context.Wishlists
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (wishlist is null)
                return new List<WishlistItemResponseDto>();

            return await context.WishlistItems
                .Where(x => x.WishlistId == wishlist.Id)
                .Select(x => new WishlistItemResponseDto
                {
                    Id = x.Id,
                    ProductId = x.ProductId,
                    ProductName = x.Product.Name,
                    Price = x.Product.Price,
                    ImageUrl = x.Product.ImageUrl
                })
                .ToListAsync();
        }
    }
}