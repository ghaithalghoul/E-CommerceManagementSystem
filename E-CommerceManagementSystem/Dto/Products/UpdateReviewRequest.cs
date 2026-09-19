using System.ComponentModel.DataAnnotations;

namespace E_CommerceManagementSystem.Dto.Products
{
    public class UpdateReviewRequest
    {
        [Required]
        [Range(1, 5)]
        public int NewRating { get; set; }
        
        [MinLength(10)]
        [MaxLength(500)]
        public string Comment { get; set; } = string.Empty;
    }
}
