using System.ComponentModel.DataAnnotations;

namespace E_CommerceManagementSystem.Dto
{
    public class LoginRequestDto
    {
        public string UsernameOrEmail { get; set; } = string.Empty;
        [Required]
        [MinLength(8)]
        [MaxLength(100)]
        public string Password { get; set; } = string.Empty;
    }
}
