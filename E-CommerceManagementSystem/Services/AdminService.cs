using Azure.Core;
using E_CommerceManagementSystem.Data;
using E_CommerceManagementSystem.Dto;
using E_CommerceManagementSystem.Dto.Admin;
using E_CommerceManagementSystem.Dto.Order;
using E_CommerceManagementSystem.Dto.Products;
using E_CommerceManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Printing;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace E_CommerceManagementSystem.Services
{
    public class AdminService(AppDbContext context,IAuditLogService auditLogService) : IAdminService
    {
        public async Task<ChangeUserRoleRequestDto?> ChangeUserRole(int userId, ChangeUserRoleRequest request)
        {
            if (request.Role is not ("Admin" or "Customer"))
                return null;
            var user = await context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user is null) return null;
            var auth = new AuditLogRequest();
            auth.UserId = user.Id;
            auth.OldValue = user.Role;
            user.Role = request.Role;
            auth.NewValue = user.Role;

            
            await auditLogService.CreateAsync(auth, AuditAction.RoleChange);
            await context.SaveChangesAsync();
            return new ChangeUserRoleRequestDto
            {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Role = user.Role,

            };

        }

        public async Task<ReviewResponse?> DeleteReview(int reviewId)
        {
            var review = await context.Reviews.FirstOrDefaultAsync(x => x.Id == reviewId);
            if (review is null) return null;
            context.Reviews.Remove(review);
            await context.SaveChangesAsync();
            return new ReviewResponse
            {
                Id = review.Id,
                UserId = review.UserId,
                ProductId = review.ProductId,
                Comment = review.Comment,
                Rating = review.Rating,
                CreatedAt = review.CreatedAt,
            };
        }

        public async Task<UserResponseDto?> DeleteUser(int userId)
        {
            var user = await context.Users
                .FirstOrDefaultAsync(x => x.Id == userId);

            if (user is null)
                return null;

            var hasOrders = await context.Orders
                .AnyAsync(x => x.UserId == userId);

            if (hasOrders)
                return null;

            context.Users.Remove(user);

            await context.SaveChangesAsync();

            return new UserResponseDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Role = user.Role,
                CreatedAt = user.CreatedAt,
                IsActive = false,
                OrdersCount = 0,
                TotalSpent = 0
            };
        }

        public async Task<PaginatedResponse<OrderResponse>> GetAllOrder(OrderFilterRequest request)
        {
            var query = context.Orders.AsQueryable();
            if (request.OrderStatus.HasValue)
            {
                query = query.Where(x => x.OrderStatus == request.OrderStatus);
            }
            if (request.UserId.HasValue)
            {
                query = query.Where(x => x.UserId== request.UserId);
            }
            var totalcount = await query.CountAsync();
            var totalpages = (int)Math.Ceiling((double)totalcount / request.pagesize);
            var skip = (request.page - 1) * request.pagesize;
            query = query.OrderByDescending(x => x.CreatedAt);
            var orders = await query.Select(x => new OrderResponse
            {
                Id=x.Id,
                OrderStatus = x.OrderStatus,
                
                CreatedAt=x.CreatedAt,
                TotalPrice = x.TotalPrice,
                UserId = x.UserId,
            }).Skip(skip)
                .Take(request.pagesize)
                .ToListAsync();
            return new PaginatedResponse<OrderResponse>
            {
                Data = orders,
                Page = request.page,
                PageSize = request.pagesize,
                TotalCount = totalcount,
                TotalPages = totalpages,
            };
        }

        public async Task<PaginatedResponse<UserResponseDto>> GetAllUsers(int page ,int pagesize)
        {
            var totalCount = await context.Users.CountAsync();

            var skip = (page - 1) * pagesize;
            var users = await context.Users.Select(x => new UserResponseDto
            {
                Id=x.Id,
                UserName=x.UserName,
                Email=x.Email,
                Role=x.Role,
                CreatedAt=x.CreatedAt,

            } ).Skip(skip)
                .Take(pagesize)
                .ToListAsync();

            int totalpages = (int)Math.Ceiling((double)totalCount / pagesize);
            
            return new PaginatedResponse<UserResponseDto>
            {
                Data = users,
                Page = page,
                PageSize = pagesize,
                TotalCount = totalCount,
                TotalPages = totalpages,
            };
            
        }

        public async Task<DashboardResponse> GetDashboard()
        {
            var totalUsers = await context.Users.CountAsync();
            var totalProducts = await context.Products.CountAsync();
            var totalOrders = await context.Orders.CountAsync();
            var totalRevenue = await context.Orders
                .Where(x => x.OrderStatus == OrderStatus.Delivered)
                .SumAsync(x => x.TotalPrice);
            var pendingOrders = await context.Orders
                .CountAsync(x => x.OrderStatus == OrderStatus.Pending);

            var cancelledOrders = await context.Orders
                .CountAsync(x => x.OrderStatus == OrderStatus.Cancelled);

            var lowStockProducts = await context.Products
                .CountAsync(x => x.stock > 0 && x.stock <= 5);

            var outOfStockProducts = await context.Products
                .CountAsync(x => x.stock == 0);
            var ordersByStatus = await context.Orders
                .GroupBy(x => x.OrderStatus)
                .Select(x => new OrderStatusCountDto
                {
                    Status = x.Key.ToString(),
                    Count = x.Count()
                })
                .ToListAsync();
            var topProducts = await context.OrderItems
                    .Where(x => x.Order.OrderStatus == OrderStatus.Delivered)
                    .GroupBy(x => new
                    {
                        x.ProductId,
                        x.Product.Name
                    })
                    .Select(x => new TopProductDto
                    {
                        ProductId = x.Key.ProductId,
                        ProductName = x.Key.Name,
                        QuantitySold = x.Sum(i => i.Quantity)
                    })
                    .OrderByDescending(x => x.QuantitySold)
                    .Take(5)
                    .ToListAsync();
            var lowStock = await context.Products
                .Where(x => x.stock > 0 && x.stock <= 5)
                .OrderBy(x => x.stock)
                .Select(x => new LowStockProductDto
                {
                    ProductId = x.ProductId,
                    ProductName = x.Name,
                    Stock = x.stock
                })
                .ToListAsync();
            return new DashboardResponse
            {
                TotalUsers = totalUsers,
                TotalProducts = totalProducts,
                TotalOrders = totalOrders,
                TotalRevenue = totalRevenue,

                PendingOrders = pendingOrders,
                CancelledOrders = cancelledOrders,

                LowStockProducts = lowStockProducts,
                OutOfStockProducts = outOfStockProducts,

                OrdersByStatus = ordersByStatus,
                
                TopProducts = topProducts,
                LowStock = lowStock
            };
        }

        public async Task<PaginatedResponse<ReviewResponse>> GetReviews(ReviewFilterRequest request)
        {
            var query = context.Reviews.AsQueryable();
            if (!string.IsNullOrWhiteSpace(request.ProductName))
            {
                query = query.Where(x => x.Product.Name.Contains(request.ProductName));
            }
            if (request.MaxRating.HasValue)
            {
                query = query.Where(x => x.Rating <= request.MaxRating.Value);
            }
            if (request.MinRating.HasValue)
            {
                query = query.Where(x => x.Rating >= request.MinRating.Value);
            }

            if(request.SortBy == "Rating")
            {
                if(request.SortOrder == "desc")
                {
                    query = query.OrderByDescending(x => x.Rating);
                }
                else
                {
                    query = query.OrderBy(x => x.Rating);

                }
            }
            else
            {
                query = query.OrderByDescending(x => x.CreatedAt);
            }
            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(
                (double)totalCount / request.pagesize);
            var skip = (request.page - 1) * request.pagesize;
            var reviews = await query.Select(x => new ReviewResponse
            {
                Id = x.Id,
                UserId = x.UserId,
                ProductId = x.ProductId,
                Rating = x.Rating,
                Comment = x.Comment,
                CreatedAt = x.CreatedAt,
            }).Skip(skip).Take(request.pagesize).ToListAsync();
            return new PaginatedResponse<ReviewResponse>
            {
                Data = reviews,
                TotalCount = totalCount,
                Page = request.page,
                PageSize = request.pagesize,
                TotalPages = totalPages,
            };
        }

        public async Task<UserResponseDto?> GetUser(int userId)
        {
            var user = await context.Users
                .FirstOrDefaultAsync(x => x.Id == userId);

            if (user is null)
                return null;

            var totalSpent = await context.OrderItems
            .Where(x => x.Order.UserId == userId &&
                        x.Order.OrderStatus == OrderStatus.Delivered)
            .SumAsync(x => x.UnitPrice * x.Quantity);

            var ordersCount = await context.Orders
                .CountAsync(x => x.UserId == userId);

            var isActive = await context.Orders
                .AnyAsync(x => x.UserId == userId &&
                               x.CreatedAt >= DateTime.UtcNow.AddMonths(-3));

            return new UserResponseDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Role = user.Role,
                CreatedAt = user.CreatedAt,
                IsActive = isActive,
                OrdersCount = ordersCount,
                TotalSpent = totalSpent
            };
        }
    }
}
