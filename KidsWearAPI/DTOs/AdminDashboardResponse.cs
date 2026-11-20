
namespace KidsWearAPI.DTOs
{
    public class AdminDashboardResponse
    {
        public int TotalProducts { get; set; }

        public int TotalCustomers { get; set; }

        public int TotalOrders { get; set; }

        public int PendingOrders { get; set; }

        public decimal TotalRevenue { get; set; }

        public int LowStockProducts { get; set; }

        public int OutOfStockProducts { get; set; }

        public List<CategorySummary> CategoryData { get; set; } = new();

        public List<MonthlySalesSummary> MonthlySales { get; set; } = new();

        public List<RecentOrderSummary> RecentOrders { get; set; } = new();
    }

    public class CategorySummary
    {
        public string Name { get; set; } = "";

        public int Value { get; set; }
    }

    public class MonthlySalesSummary
    {
        public string Month { get; set; } = "";

        public decimal Revenue { get; set; }
    }

    public class RecentOrderSummary
    {
        public int Id { get; set; }

        public string CustomerName { get; set; } = "";

        public DateTime CreatedAt { get; set; }

        public decimal Total { get; set; }

        public string Status { get; set; } = "";
    }
}

