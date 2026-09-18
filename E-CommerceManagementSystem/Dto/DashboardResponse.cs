namespace E_CommerceManagementSystem.Dto
{
    public class DashboardResponse
    {
        public int TotalUsers { get; set; }
        public int TotalProducts { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }

        public int PendingOrders { get; set; }
        public int CancelledOrders { get; set; }

        public int LowStockProducts { get; set; }
        public int OutOfStockProducts { get; set; }

        public List<OrderStatusCountDto> OrdersByStatus { get; set; } = new();
        public List<SalesOverTimeDto> SalesOverTime { get; set; } = new();
        public List<TopProductDto> TopProducts { get; set; } = new();
        public List<LowStockProductDto> LowStock { get; set; } = new();

    }
}
