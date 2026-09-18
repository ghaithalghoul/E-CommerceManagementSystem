using E_CommerceManagementSystem.Dto;
using E_CommerceManagementSystem.Dto.Category;
using E_CommerceManagementSystem.Models;

namespace E_CommerceManagementSystem.Services
{
    public interface ICategoriesService
    {
        Task<List<CategoryResponseDto>> GetCategoriesAsync();
        Task<CategoryResponseDto?> GetCategoryAsync(int id);
        Task<CategoryResponseDto?> AddCategorieAsync(CategoryDto request);
        Task<CategoryResponseDto?> UpdateCategorieAsync(int id,CategoryDto request);
        Task<CategoryResponseDto?> DeleteCategorieAsync(int id);
    }
}
