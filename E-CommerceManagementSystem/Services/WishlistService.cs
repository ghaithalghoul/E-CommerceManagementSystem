using E_CommerceManagementSystem.Data;
using E_CommerceManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace E_CommerceManagementSystem.Services
{
    public class WishlistService(AppDbContext context) : IWishlistService
    {
        public async Task<WishlistItem?> AddWishlistItemAsync(int userId, int productId)
        {
            var wishlist = await context.Wishlists
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (wishlist is null)
                return null;

            var productExists = await context.Products
                .AnyAsync(x => x.ProductId == productId);

            if (!productExists)
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

            return wishlistItem;
        }

        public async Task<WishlistItem?> DeleteWishlistItemAsync(int userId, int productId)
        {
            var wishlist = await context.Wishlists.FirstOrDefaultAsync(x => x.UserId == userId);
            if (wishlist == null) return null;
            var wishlistitem = await context.WishlistItems.FirstOrDefaultAsync(x => x.ProductId == productId 
            && x.WishlistId == wishlist.Id);
            if (wishlistitem is null) return null;
            context.WishlistItems.Remove(wishlistitem);
            await context.SaveChangesAsync();
            return wishlistitem;
        }

        public async Task<List<WishlistItem>> GetWishlistAsync(int userId)
        {
            var wishlist = await context.Wishlists.FirstOrDefaultAsync(x => x.UserId == userId);
            if (wishlist is null)
                return new List<WishlistItem>();
            var wishlistitems = await context.WishlistItems.Where(x => x.WishlistId == wishlist.Id).ToListAsync();
            return wishlistitems;
        }
    }
}
