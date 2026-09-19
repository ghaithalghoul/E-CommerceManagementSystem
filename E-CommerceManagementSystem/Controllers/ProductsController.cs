using E_CommerceManagementSystem.Dto;
using E_CommerceManagementSystem.Dto.Products;
using E_CommerceManagementSystem.Extensions;
using E_CommerceManagementSystem.Migrations;
using E_CommerceManagementSystem.Models;
using E_CommerceManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_CommerceManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController(IProductsService productsService) : ControllerBase
    {
        [HttpGet("all-products")]
        public async Task<ActionResult<List<ProductDto>>> GetAllProducts()
        {
            var products = await productsService.GetAllProduct();
            
            return Ok(products);
        }
        [HttpGet]
        public async Task<ActionResult<PaginatedResponse<ProductDto>>> GetProducts([FromQuery] ProductFilterRequest request) 
        {
            var products = await productsService.GetProductsAsync(request);
            return Ok(products);
        }
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductDto>> GetProduct(int id)
        {
            var product = await productsService.GetProduct(id);
            if (product == null) return NotFound();
            return Ok(product);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("add-product")]
        public async Task<IActionResult> AddProduct(CreateProductRequest request)
        {
            var result = await productsService.AddProduct(request);
            if(result == null) return BadRequest();
            return StatusCode(201, result);
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("update-product/{id:int}")]
        public async Task<ActionResult<ProductDto>> UpdateProduct(int id, UpdateProductRequest request)
        {
            var adminId = User.GetUserId();
            var result = await productsService.UpdateProduct(adminId,id, request);
            if (result == null) return NotFound();
            return Ok(result);
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteProduct(int id) 
        {
            var result = await productsService.DeleteProduct(id);
            if (result == null) return NotFound();
            return Ok(result);
        }
        
        [HttpGet("product-review/{productId:int}")]
        public async Task<ActionResult<PaginatedResponse<ReviewResponseDto>>> GetReviews(int productId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await productsService.GetReviews(productId, page, pageSize);
            if (result is null) return BadRequest();
            return Ok(result);
        }
        [Authorize]
        [HttpPost("add-review")]
        public async Task<ActionResult<ReviewResponseDto?>> AddReview(ReviewRequest request)
        {
            var userId = User.GetUserId();
            var result = await productsService.AddReview(userId, request);
            if (result is null) return BadRequest();
            return Ok(result);
        }
        [Authorize]
        [HttpPut("update-review/{reviewId:int}")]
        public async Task<ActionResult<ReviewResponseDto?>> UpdateReview( int reviewId, UpdateReviewRequest request)
        {
            var userId = User.GetUserId();
            var result = await productsService.UpdateReview(userId,reviewId,  request);
            if (result is null) return BadRequest();
            return Ok(result);
        }
        [Authorize]
        [HttpDelete("reviews/{reviewId:int}")]
        public async Task<ActionResult<ReviewResponseDto?>> DeleteReview(int reviewId)
        {
            var userId = User.GetUserId();
            var result = await productsService.DeleteReview(userId, reviewId);
            if (result is null) return BadRequest();
            return Ok(result);
        }
    }
}
