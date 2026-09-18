namespace E_CommerceManagementSystem.Dto
{
    public class ChangeUserRoleRequestDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;

    }
}
