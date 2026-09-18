using E_CommerceManagementSystem.Dto;
using E_CommerceManagementSystem.Dto.Products;
using E_CommerceManagementSystem.Models;
using System.Drawing.Printing;

namespace E_CommerceManagementSystem.Services
{
    public interface IProductsService
    {
        Task<List<ProductDto>> GetAllProduct();
        Task<ProductDto?> GetProduct(int productId);
        Task<ProductDto?> AddProduct(CreateProductRequest request);
        Task<ProductDto?> UpdateProduct(int adminId,int productId, UpdateProductRequest request);
        Task<ProductDto?> DeleteProduct(int productId);
        Task<PaginatedResponse<ProductDto>> GetProductsAsync(ProductFilterRequest request);
        Task<Models.Review?> AddReview(int userId, ReviewRequest request);
        Task<PaginatedResponse<Models.Review>> GetReviews(
            int productId,
            int page,
            int pageSize);
        Task<Models.Review?> UpdateReview(int userId, int reviewId, UpdateReviewRequest request);
        Task<Models.Review?> DeleteReview(int userId, int reviewId);
        

    }
}
