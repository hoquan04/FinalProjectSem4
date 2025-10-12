using AdminWeb.Areas.Admin.Models;
using System.Net.Http.Json;

namespace AdminWeb.Areas.Admin.Data.Services
{
    public class DashboardService
    {
        private readonly HttpClient _httpClient;
        private const string ApiUrl = "http://localhost:7245/api/dashboard";

        public DashboardService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ApiResponse<DashboardStatsViewModel>> GetDashboardAsync()
        {
            var response = await _httpClient.GetAsync(ApiUrl);
            if (!response.IsSuccessStatusCode)
            {
                return new ApiResponse<DashboardStatsViewModel>
                {
                    Success = false,
                    Message = $"HTTP {response.StatusCode}"
                };
            }

            var result = await response.Content.ReadFromJsonAsync<ApiResponse<DashboardStatsViewModel>>();
            return result ?? new ApiResponse<DashboardStatsViewModel> { Success = false, Message = "Không parse được JSON" };
        }
    }

    public class DashboardStatsViewModel
    {
        public int TotalUsers { get; set; }
        public int TotalProducts { get; set; }
        public int TotalCategories { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }

        public List<OrderStatusStat> OrderStatusStats { get; set; } = new();
        public List<MonthlyRevenueDto> MonthlyRevenue { get; set; } = new();
        public List<Order> RecentOrders { get; set; } = new();
    }

    public class OrderStatusStat
    {
        public string Status { get; set; } = "";
        public int Count { get; set; }
    }

    public class MonthlyRevenueDto
    {
        public string Month { get; set; } = "";
        public decimal Revenue { get; set; }
    }
}
