using E_CommerceManagementSystem.Data;
using E_CommerceManagementSystem.Dto;
using E_CommerceManagementSystem.Dto.Order;
using E_CommerceManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace E_CommerceManagementSystem.Services
{
    public class OrderService(AppDbContext context,IAuditLogService auditLogService) : IOrderService
    {
        public async Task<OrderResponse?> CancelOrder(int userId, int orderId)
        {
            await using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                var order = await context.Orders
                    .Include(x => x.OrderItems)
                    .FirstOrDefaultAsync(x =>
                        x.Id == orderId &&
                        x.UserId == userId);

                if (order is null)
                    return null;

                if (order.OrderStatus != OrderStatus.Processing &&
                    order.OrderStatus != OrderStatus.Pending)
                {
                    return null;
                }

                var productIds = order.OrderItems
                    .Select(x => x.ProductId)
                    .Distinct()
                    .ToList();

                var products = await context.Products
                    .Where(x => productIds.Contains(x.ProductId))
                    .ToListAsync();

                var productsDictionary = products
                    .ToDictionary(x => x.ProductId);

                foreach (var item in order.OrderItems)
                {
                    if (!productsDictionary.TryGetValue(item.ProductId, out var product))
                        return null;

                    product.stock += item.Quantity;
                }

                order.OrderStatus = OrderStatus.Cancelled;

                await context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new OrderResponse
                {
                    Id = order.Id,
                    UserId = order.UserId,
                    OrderStatus = order.OrderStatus,
                    TotalPrice = order.TotalPrice,
                    CreatedAt = order.CreatedAt,
                    Items = order.OrderItems.Select(x => new OrderItemResponseDto
                    {
                        ProductId = x.ProductId,
                        Quantity = x.Quantity,
                        UnitPrice = x.UnitPrice,
                        Total = x.UnitPrice * x.Quantity
                    }).ToList()
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<OrderResponse?> CreateOrder(int userId)
        {
            await using var transaction =
                await context.Database.BeginTransactionAsync();

            try
            {
                var cart = await context.Carts
                    .FirstOrDefaultAsync(x => x.UserId == userId);

                if (cart is null)
                    return null;

                var cartItems = await context.CartItems
                    .Where(x => x.CartId == cart.Id)
                    .ToListAsync();

                if (!cartItems.Any())
                    return null;

                var productIds = cartItems
                    .Select(x => x.ProductId)
                    .Distinct()
                    .ToList();

                var products = await context.Products
                    .Where(x => productIds.Contains(x.ProductId))
                    .ToListAsync();

                var productsDictionary = products
                    .ToDictionary(x => x.ProductId);

                var order = new Order
                {
                    UserId = userId,
                    OrderStatus = OrderStatus.Pending,
                    CreatedAt = DateTime.UtcNow
                };

                decimal totalPrice = 0;

                foreach (var item in cartItems)
                {
                    if (!productsDictionary.TryGetValue(item.ProductId, out var product))
                        return null;

                    if (product.stock < item.Quantity)
                        return null;

                    var orderItem = new OrderItem
                    {
                        ProductId = product.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = product.Price
                    };

                    order.OrderItems.Add(orderItem);

                    totalPrice += product.Price * item.Quantity;

                    product.stock -= item.Quantity;
                }

                order.TotalPrice = totalPrice;

                context.Orders.Add(order);
                context.CartItems.RemoveRange(cartItems);

                await context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new OrderResponse
                {
                    Id = order.Id,
                    UserId = order.UserId,
                    OrderStatus = order.OrderStatus,
                    TotalPrice = order.TotalPrice,
                    CreatedAt = order.CreatedAt,
                    Items = order.OrderItems.Select(x => new OrderItemResponseDto
                    {
                        ProductId = x.ProductId,
                        Quantity = x.Quantity,
                        UnitPrice = x.UnitPrice,
                        Total = x.UnitPrice * x.Quantity
                    }).ToList()
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<List<OrderResponse>> GetAllOrders(int userId)
        {
            return await context.Orders
                .Where(x => x.UserId == userId)
                .Include(x => x.OrderItems)
                .Select(x => new OrderResponse
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    OrderStatus = x.OrderStatus,
                    TotalPrice = x.TotalPrice,
                    CreatedAt = x.CreatedAt,
                    Items = x.OrderItems.Select(item => new OrderItemResponseDto
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        Total = item.UnitPrice * item.Quantity
                    }).ToList()
                })
                .ToListAsync();
        }

        public async Task<OrderResponse?> GetOrder(int userId, int orderId)
        {
            var order = await context.Orders
                .Include(x => x.OrderItems)
                .FirstOrDefaultAsync(x =>
                    x.Id == orderId &&
                    x.UserId == userId);

            if (order is null)
                return null;

            return new OrderResponse
            {
                Id = order.Id,
                UserId = order.UserId,
                OrderStatus = order.OrderStatus,
                TotalPrice = order.TotalPrice,
                CreatedAt = order.CreatedAt,
                Items = order.OrderItems.Select(x => new OrderItemResponseDto
                {
                    ProductId = x.ProductId,
                    Quantity = x.Quantity,
                    UnitPrice = x.UnitPrice,
                    Total = x.UnitPrice * x.Quantity
                }).ToList()
            };
        }

        public async Task<OrderResponse?> UpdateOrderStatus(
            int adminId,
            UpdateOrderRequest request)
                {
            var order = await context.Orders
                .Include(x => x.OrderItems)
                .FirstOrDefaultAsync(x => x.Id == request.OrderId);

            if (order is null || order.OrderStatus == request.OrderStatus)
                return null;

            var auth = new AuditLogRequest
            {
                UserId = adminId,
                OldValue = order.OrderStatus.ToString()
            };

            order.OrderStatus = request.OrderStatus;

            auth.NewValue = order.OrderStatus.ToString();

            await auditLogService.CreateAsync(
                auth,
                AuditAction.OrderStatusChange);

            await context.SaveChangesAsync();

            return new OrderResponse
            {
                Id = order.Id,
                UserId = order.UserId,
                OrderStatus = order.OrderStatus,
                TotalPrice = order.TotalPrice,
                CreatedAt = order.CreatedAt,
                Items = order.OrderItems.Select(x => new OrderItemResponseDto
                {
                    ProductId = x.ProductId,
                    Quantity = x.Quantity,
                    UnitPrice = x.UnitPrice,
                    Total = x.UnitPrice * x.Quantity
                }).ToList()
            };
        }
    }
}
