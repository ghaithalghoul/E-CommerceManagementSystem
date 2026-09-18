using E_CommerceManagementSystem.Dto;
using E_CommerceManagementSystem.Dto.Category;
using E_CommerceManagementSystem.Models;
using E_CommerceManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_CommerceManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController(ICategoriesService categoriesService) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<CategoryResponseDto>>> GetAllCategories()
        {
            var result = await categoriesService.GetCategoriesAsync();
            return Ok(result);
        }
        [HttpGet("{id:int}")]
        public async Task<ActionResult<CategoryResponseDto>> GetCategory(int id)
        {
            var result = await categoriesService.GetCategoryAsync(id);
            if (result is null) return NotFound();
            return Ok(result);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<CategoryResponseDto>> AddCategory(CategoryDto request)
        {
            var resuelt = await categoriesService.AddCategorieAsync(request);
            if(resuelt is null) return BadRequest();
            return CreatedAtAction(
                    nameof(GetCategory),
                    new { id = resuelt.Id },
                    resuelt);
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        public async Task<ActionResult<CategoryResponseDto>> UpdateCategory(int id,CategoryDto request)
        {
            var result = await categoriesService.UpdateCategorieAsync(id, request);
            if(result is null) return NotFound();
            return Ok(result);
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<CategoryResponseDto>> DeleteCategory(int id)
        {
            var result = await categoriesService.DeleteCategorieAsync(id);
            if (result is null) return NotFound();
            return Ok(result);
        }
    }
}
