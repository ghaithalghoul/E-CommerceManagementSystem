using System.ComponentModel.DataAnnotations;

namespace E_CommerceManagementSystem.Dto.Products
{
    public class ReviewRequest
    {
        [Required]
        [Range(1, int.MaxValue)]
        public  int ProductId { get; set; }
        [Required]
        [Range (1, 5)]
        public  int Rating { get; set; }
        
        [MinLength(10)]
        [MaxLength(500)]
        public string Comment { get; set; } = string.Empty;
    }
}
