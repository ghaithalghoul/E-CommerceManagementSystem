using System.ComponentModel.DataAnnotations;

namespace E_CommerceManagementSystem.Dto
{
    public class LoginRequestDto
    {
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        [Required]
        [MinLength(8)]
        [MaxLength(100)]
        public string Password { get; set; } = string.Empty;
    }
}
