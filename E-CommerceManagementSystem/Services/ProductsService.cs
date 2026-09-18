using E_CommerceManagementSystem.Data;
using E_CommerceManagementSystem.Dto;
using E_CommerceManagementSystem.Dto.Products;
using E_CommerceManagementSystem.Migrations;
using E_CommerceManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace E_CommerceManagementSystem.Services
{
    public class ProductsService(AppDbContext dbContext,IAuditLogService auditLogService) : IProductsService
    {
        public async Task<ProductDto?> AddProduct(CreateProductRequest request)
        {
            var product = new Product();
            product.Name = request.Name;
            product.Description = request.Description;
            product.Price = request.Price;
            product.CategoryId = request.CategoryId;
            product.stock = request.Stock;
            product.ImageUrl = request.ImageUrl;
            product.CreatedAt = DateTime.UtcNow;
            dbContext.Products.Add(product);
            await dbContext.SaveChangesAsync();
            return new ProductDto
            {
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.stock,
                ImageUrl = product.ImageUrl,
                CategoryId = product.CategoryId,
            };
        }

        public async Task<Models.Review?> AddReview(int userId, ReviewRequest request)
        {
            var hasPurchased = await dbContext.Orders
            .AnyAsync(order =>
                order.UserId == userId &&
                order.OrderItems.Any(item =>
                    item.ProductId == request.ProductId));
            if (!hasPurchased || request.Rating < 1 || request.Rating > 5) return null;
            var alreadyReviewed = await dbContext.Reviews
                    .AnyAsync(x =>
                        x.UserId == userId &&
                        x.ProductId == request.ProductId);

            if (alreadyReviewed)
                return null;
            var review = new Models.Review();
            review.UserId = userId;
            review.ProductId = request.ProductId;
            review.Rating = request.Rating;
            review.Comment = request.Comment;
            review.CreatedAt = DateTime.UtcNow;
            dbContext.Reviews.Add(review);
            await dbContext.SaveChangesAsync();
            return review;
            
        }

        public async Task<ProductDto?> DeleteProduct(int productId)
        {
            var product = await dbContext.Products.FirstOrDefaultAsync(x => x.ProductId == productId);
            if (product == null) return null;
            dbContext.Products.Remove(product);
            await dbContext.SaveChangesAsync();
            return new ProductDto
            {
                Name = product.Name,
                Price = product.Price,
                Description = product.Description,
                ImageUrl = product.ImageUrl,
                CategoryId = product.CategoryId,
                Stock = product.stock
            };
        }

        public async Task<Models.Review?> DeleteReview(int userId, int reviewId)
        {
            var review = await dbContext.Reviews.FirstOrDefaultAsync(x => x.Id == reviewId && x.UserId == userId);
            if(review == null) return null;
            dbContext.Reviews.Remove(review);
            await dbContext.SaveChangesAsync();
            return review;
        }

        public async Task<List<ProductDto>> GetAllProduct()
        {
            return await dbContext.Products.Select(x => new ProductDto
            {
                ProductId = x.ProductId,
                Name = x.Name,
                Description = x.Description,
                Price = x.Price,
                Stock = x.stock,
                ImageUrl = x.ImageUrl,
                CategoryId = x.CategoryId
            }).ToListAsync();

        }

        public async Task<ProductDto?> GetProduct(int productId)
        {
            var product = await dbContext.Products.FirstOrDefaultAsync(x => x.ProductId == productId);
            if (product == null) return null;
            return new ProductDto
            {
                Name = product.Name,
                Price = product.Price,
                Stock = product.stock,
                Description = product.Description,
                ImageUrl= product.ImageUrl,
                CategoryId= product.CategoryId
            };
        }

        public async Task<PaginatedResponse<ProductDto>> GetProductsAsync(ProductFilterRequest request)
        {
            var query = dbContext.Products.AsQueryable();
            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                query = query.Where(x => x.Name.Contains(request.Search));
            }
            if (request.MinPrice.HasValue)
            {
                query = query.Where(x => x.Price >= request.MinPrice.Value);
            }
            if(request.MaxPrice.HasValue)
            {
                query = query.Where(x => x.Price <= request.MaxPrice.Value);
            }
            if(request.CategoryId.HasValue)
            {
                query = query.Where(x => x.CategoryId == request.CategoryId.Value);
            }
            switch (request.SortBy)
            {
                case "price":
                    if (request.SortOrder == "desc")
                    {
                        query = query.OrderByDescending(x => x.Price);
                    }
                    else
                    {
                        query = query.OrderBy(x => x.Price);
                    }
                    break;
                case "name":
                    if (request.SortOrder == "desc")
                    {
                        query = query.OrderByDescending(x => x.Name);
                    }
                    else
                    {
                        query = query.OrderBy(x => x.Name);
                    }
                    break;
                case "createdAt":
                    if (request.SortOrder == "desc")
                    {
                        query = query.OrderByDescending(x => x.CreatedAt);
                    }
                    else
                    {
                        query = query.OrderBy(x => x.CreatedAt);
                    }
                    break;
                case "stock":
                    if (request.SortOrder == "desc")
                    {
                        query = query.OrderByDescending(x => x.stock);
                    }
                    else
                    {
                        query = query.OrderBy(x => x.stock);
                    }
                    break;
                default:
                    query = query.OrderByDescending(x => x.CreatedAt);
                    break;


            }
            
            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(
                (double)totalCount / request.PageSize);
            var skip = (request.Page - 1) * request.PageSize;
            var products = await query
                .Select(x => new ProductDto
                {
                    ProductId = x.ProductId,
                    Name = x.Name,
                    Description = x.Description,
                    Price = x.Price,
                    Stock = x.stock,
                    ImageUrl = x.ImageUrl,
                    CategoryId = x.CategoryId
                })
                .Skip(skip)
                .Take(request.PageSize)
                .ToListAsync();
            return new PaginatedResponse<ProductDto>
            {
                Data = products,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = totalPages,
            };
        }

        public async Task<PaginatedResponse<Models.Review>> GetReviews(
            int productId,
            int page,
            int pageSize)
        {
            var query = dbContext.Reviews
                .Where(x => x.ProductId == productId);

            var totalCount = await query.CountAsync();

            var totalPages = (int)Math.Ceiling(
                (double)totalCount / pageSize);

            var skip = (page - 1) * pageSize;

            var reviews = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResponse<Models.Review>
            {
                Data = reviews,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };
        }

        public async Task<ProductDto?> UpdateProduct(int adminId,int productId, UpdateProductRequest request)
        {
            var product = await dbContext.Products.FirstOrDefaultAsync(x => x.ProductId == productId);
            if (product == null) return null;
            product.Name = request.Name;
            product.Description = request.Description;
            product.Price = request.Price;
            product.stock = request.Stock;
            product.CategoryId = request.CategoryId;
            product.UpdatedAt = DateTime.UtcNow;
            var auth = new AuditLogRequest();
            auth.UserId = adminId;
            await auditLogService.CreateAsync(auth, AuditAction.ProductChange);
            await dbContext.SaveChangesAsync();
            return new ProductDto
            {
                Name = product.Name,
                Price = product.Price,
                Stock = product.stock,
                Description = product.Description,
                ImageUrl = product.ImageUrl,
                CategoryId = product.CategoryId
            };
        }

        public async Task<Models.Review?> UpdateReview(int userId, int reviewId, UpdateReviewRequest request)
        {
            if (request.NewRating < 1 || request.NewRating > 5)
                return null;

            var review = await dbContext.Reviews
                .FirstOrDefaultAsync(x =>
                    x.Id == reviewId &&
                    x.UserId == userId);

            if (review is null)
                return null;

            review.Rating = request.NewRating;
            review.Comment = request.Comment;

            await dbContext.SaveChangesAsync();

            return review;
        }
    }
}
