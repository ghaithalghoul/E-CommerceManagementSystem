using System.ComponentModel.DataAnnotations;

namespace E_CommerceManagementSystem.Dto
{
    public class RegisterRequestDto
    {
        [Required]
        [MinLength(5)]
        [MaxLength(50)]
        public string UserName { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        [MinLength(8)]
        [MaxLength(100)]
        public string Password { get; set; } = string.Empty;
    }
}
