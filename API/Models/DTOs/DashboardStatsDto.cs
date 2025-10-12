// Models/DTOs/DashboardStatsDto.cs
namespace API.Models.DTOs
{
    public class DashboardStatsDto
    {
        public int TotalUsers { get; set; }
        public int TotalProducts { get; set; }
        public int TotalCategories { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }

        public List<OrderStatusStat> OrderStatusStats { get; set; } = new();
        public List<MonthlyRevenueDto> MonthlyRevenue { get; set; } = new();
        public List<RecentOrderDto> RecentOrders { get; set; } = new();
    }

    public class OrderStatusStat
    {
        // Để đơn giản, giữ string; bạn có thể đổi sang int nếu muốn
        public string Status { get; set; } = "";
        public int Count { get; set; }
    }

    public class MonthlyRevenueDto
    {
        public string Month { get; set; } = "";
        public decimal Revenue { get; set; }
    }

    // 👇 DTO rút gọn cho bảng "Đơn hàng mới nhất"
    public class RecentOrderDto
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public int Status { get; set; }          // ✅ TRẢ SỐ (int) thay vì chuỗi
        public string? RecipientName { get; set; }
    }
}
