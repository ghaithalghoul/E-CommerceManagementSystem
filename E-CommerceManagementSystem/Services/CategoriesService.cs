using Azure.Core;
using E_CommerceManagementSystem.Data;
using E_CommerceManagementSystem.Dto;
using E_CommerceManagementSystem.Dto.Category;
using E_CommerceManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace E_CommerceManagementSystem.Services
{
    public class CategoriesService(AppDbContext context) : ICategoriesService
    {
        public async Task<CategoryResponseDto?> AddCategorieAsync(CategoryDto request)
        {
            var category = new Category();
            category.Name = request.Name;
            context.Categories.Add(category);
            await context.SaveChangesAsync();
            return new CategoryResponseDto
            {
                Id = category.Id,
                Name = category.Name,
            };
        }

        public async Task<CategoryResponseDto?> DeleteCategorieAsync(int id)
        {
            var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);
            if (category is null) return null;
            var products = await context.Products.AnyAsync(x => x.CategoryId == category.Id);
            if (products) return null;
            context.Categories.Remove(category);
            await context.SaveChangesAsync();
            return new CategoryResponseDto
            {
                Id = category.Id,
                Name = category.Name,
            }; 
        }

        public async Task<List<CategoryResponseDto>> GetCategoriesAsync()
        {
            return await context.Categories.Select( x => new CategoryResponseDto
            {
                Id = x.Id,
                Name= x.Name,
            }).ToListAsync();

        }

        public async Task<CategoryResponseDto?> GetCategoryAsync(int id)
        {
            var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);
            if (category is null) return null;
            return new CategoryResponseDto
            {
                Id = category.Id,
                Name = category.Name,
            };
        }

        public async Task<CategoryResponseDto?> UpdateCategorieAsync(int id, CategoryDto request)
        {
            var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);
            if (category is null) return null;
            category.Name = request.Name;
            await context.SaveChangesAsync();
            return new CategoryResponseDto
            {
                Id = category.Id,
                Name = category.Name,
            };
        }
    }
}
