using E_CommerceManagementSystem.Data;
using E_CommerceManagementSystem.Dto;
using E_CommerceManagementSystem.Dto.Order;
using E_CommerceManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace E_CommerceManagementSystem.Services
{
    public class OrderService(AppDbContext context,IAuditLogService auditLogService) : IOrderService
    {
        public async Task<Order?> CancelOrder(int userId, int OrderId)
        {
            await using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var order = await context.Orders.FirstOrDefaultAsync(x => x.Id == OrderId && x.UserId == userId);
                if (order == null) return null;
                if (order.OrderStatus != OrderStatus.Processing && order.OrderStatus != OrderStatus.Pending)
                {
                    return null;
                }
                var orderItems = await context.OrderItems.Where(x => x.OrderId == order.Id).ToListAsync();
                foreach(var item in orderItems)
                {
                    var product = await context.Products.FirstOrDefaultAsync(x => x.ProductId == item.ProductId);
                    if (product is null) return null;
                    product.stock += item.Quantity;
                }
                order.OrderStatus = OrderStatus.Cancelled;
                context.OrderItems.RemoveRange(orderItems);
                await context.SaveChangesAsync();
                await transaction.CommitAsync();
                return order;

            }
            catch
            {
                await transaction.RollbackAsync(); 
                throw;
            }
            
        }

        public async Task<Order?> CreateOrder(int userId)
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

                var order = new Order
                {
                    UserId = userId,
                    OrderStatus = OrderStatus.Pending,
                    CreatedAt = DateTime.UtcNow
                };

                decimal totalPrice = 0;

                foreach (var item in cartItems)
                {
                    var product = await context.Products
                        .FirstOrDefaultAsync(x => x.ProductId == item.ProductId);

                    if (product is null)
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

                return order;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<List<Order>> GetAllOrders(int userId)
        {
            return await context.Orders.Where(x => x.UserId == userId).ToListAsync();
             
        }

        public async Task<Order?> GetOrder(int userId, int OrderId)
        {
            var order = await context.Orders
            .Include(x => x.OrderItems)
            .FirstOrDefaultAsync(x =>
                x.Id == OrderId &&
                x.UserId == userId);
            if (order == null) return null;
            return order;
        }

        public async Task<Order?> UpdateOrderStatus(int adminId, UpdateOrderRequest request)
        {
            var order = await context.Orders.FirstOrDefaultAsync(x => x.Id == request.OrderId);
            if (order is null || order.OrderStatus == request.OrderStatus) return null;
            var auth = new AuditLogRequest();
            auth.UserId = adminId;
            auth.OldValue = order.OrderStatus.ToString();
            order.OrderStatus = request.OrderStatus;
            auth.OldValue = order.OrderStatus.ToString();
            await auditLogService.CreateAsync(auth, AuditAction.OrderStatusChange);
            await context.SaveChangesAsync();
            return order;
        }
    }
}
