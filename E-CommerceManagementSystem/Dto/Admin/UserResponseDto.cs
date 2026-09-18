namespace E_CommerceManagementSystem.Dto.Admin
{
    public class UserResponseDto
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = "Customer";
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }

        public int OrdersCount { get; set; }

        public decimal TotalSpent { get; set; }
    }
}
