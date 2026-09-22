using E_CommerceManagementSystem.Data;
using E_CommerceManagementSystem.Dto;
using E_CommerceManagementSystem.Dto.Cart;
using E_CommerceManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace E_CommerceManagementSystem.Services
{
    public class CartService(AppDbContext context) : ICartService
    {

        public async Task<CartItemResponseDto?> AddCartIteme(int userId, AddCartItemRequest request)
        {
            var cart = await context.Carts
                  .FirstOrDefaultAsync(x =>  x.UserId == userId);

            if (cart == null)
                return null;
            var product = await context.Products.FirstOrDefaultAsync(x => x.ProductId == request.ProductId);
            if(product == null) return null;
            var existitem = await context.CartItems.FirstOrDefaultAsync(x => x.ProductId == request.ProductId &&
            x.CartId == cart.Id);
            if(existitem == null)
            {
                var cartitem = new CartItem();
                cartitem.CartId = cart.Id;
                cartitem.ProductId = request.ProductId;
                cartitem.Quantity = request.Quantity;
                cartitem.Price = product.Price;
                context.CartItems.Add(cartitem);
                await context.SaveChangesAsync();
                return new CartItemResponseDto
                {
                    Id = cartitem.Id,
                    ProductId = cartitem.ProductId,
                    ProductName = product.Name,
                    ImageUrl = product.ImageUrl,
                    Quantity = cartitem.Quantity,
                    Price = cartitem.Price,
                    Total = cartitem.Price * cartitem.Quantity
                };
            }
            existitem.Quantity += request.Quantity;
            await context.SaveChangesAsync();
            return new CartItemResponseDto
            {
                Id = existitem.Id,
                ProductId = existitem.ProductId,
                ProductName = product.Name,
                ImageUrl = product.ImageUrl,
                Quantity = existitem.Quantity,
                Price = existitem.Price,
                Total = existitem.Price * existitem.Quantity
            };

        }


        public async Task<CartResponse?> DeleteCart(int userId)
        {
            var cart = await context.Carts
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (cart == null)
                return null;

            var cartItems = await context.CartItems
                .Where(x => x.CartId == cart.Id)
                .ToListAsync();

            var responseItems = cartItems
                .Select(x => new CartItemResponseDto
                {
                    Id = x.Id,
                    ProductId = x.ProductId,
                    Quantity = x.Quantity,
                    Price = x.Price,
                    Total = x.Price * x.Quantity
                })
                .ToList();

            var total = responseItems.Sum(x => x.Total);

            context.CartItems.RemoveRange(cartItems);

            await context.SaveChangesAsync();

            return new CartResponse
            {
                CartId = cart.Id,
                Items = responseItems,
                Total = total
            };
        }

        public async Task<CartItemResponseDto?> DeleteCartItem(int id, int userId)
        {
            var cart = await context.Carts
                  .FirstOrDefaultAsync(x => x.UserId == userId);

            if (cart == null)
                return null;
            var cartitem = await context.CartItems.FirstOrDefaultAsync(x => x.Id == id &&
                x.CartId == cart.Id);
            if(cartitem == null) return null;
            context.CartItems.Remove(cartitem);
            await context.SaveChangesAsync();
            return new CartItemResponseDto
            {
                Id = cartitem.Id,
                ProductId = cartitem.ProductId,
                Quantity = cartitem.Quantity,
                Price = cartitem.Price,
                Total = cartitem.Price * cartitem.Quantity
            };
        }

        public async Task<CartResponse?> GetCart(int userId)
        {
            var cart = await context.Carts
                  .FirstOrDefaultAsync(x =>  x.UserId == userId);

            if (cart == null)
                return null;

            var cartItems = await context.CartItems
                .Where(x => x.CartId == cart.Id)
                .Select(x => new CartItemResponseDto
                {
                    Id = x.Id,
                    ProductId = x.ProductId,
                    ProductName = x.Product.Name,
                    ImageUrl = x.Product.ImageUrl,
                    Quantity = x.Quantity,
                    Price = x.Price,
                    Total = x.Price * x.Quantity
                }).ToListAsync();

            var total = cartItems.Sum(x => x.Price * x.Quantity);

            return new CartResponse
            {
                CartId = cart.Id,
                Items = cartItems,
                Total = total
            };
        }

        public async Task<CartItemResponseDto?> UpdateCartItme(int userId, UpdateCartItmeRequest request,int cartitemId)
        {
            var cart = await context.Carts
                  .FirstOrDefaultAsync(x => x.UserId == userId);

            if (cart == null)
                return null;
            var cartitem = await context.CartItems.FirstOrDefaultAsync(x => x.Id == cartitemId &&
        x.CartId == cart.Id);
            if (cartitem == null) return null;
            cartitem.Quantity = request.Quantity;
            await context.SaveChangesAsync();
            return new CartItemResponseDto
            {
                Id = cartitem.Id,
                ProductId = cartitem.ProductId,
                Quantity = cartitem.Quantity,
                Price = cartitem.Price,
                Total = cartitem.Price * cartitem.Quantity
            };
        }

        
    }
}
