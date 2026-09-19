using System.ComponentModel.DataAnnotations;

namespace E_CommerceManagementSystem.Dto.Category
{
    public class CategoryDto
    {
        [Required]
        [MinLength(3)]
        public string Name { get; set; } = string.Empty;
    }
}
