using API.Data;
using API.Models.DTOs;
using API.Repositories.RestAPI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly DataContext _context;
        public DashboardController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetDashboard()
        {
            var totalUsers = await _context.Users.CountAsync();
            var totalProducts = await _context.Products.CountAsync();
            var totalCategories = await _context.Categories.CountAsync();
            var totalOrders = await _context.Orders.CountAsync();

            var totalRevenue = await _context.Orders.SumAsync(o => (decimal?)o.TotalAmount ?? 0);

            // Đếm trạng thái: trả về label là số (chuỗi số) để tránh enum-string
            var statusStats = await _context.Orders
                .GroupBy(o => o.Status)
                .Select(g => new OrderStatusStat
                {
                    Status = ((int)g.Key).ToString(),
                    Count = g.Count()
                })
                .ToListAsync();

            var now = DateTime.Now;
            var from = now.AddMonths(-11);

            // Bước 1: Query raw
            var monthlyRevenueRaw = await _context.Orders
                .Where(o => o.OrderDate >= from)
                .GroupBy(o => new { o.OrderDate.Year, o.OrderDate.Month })
                .Select(g => new
                {
                    g.Key.Year,
                    g.Key.Month,
                    Revenue = g.Sum(o => (decimal?)o.TotalAmount ?? 0)
                })
                .OrderBy(g => g.Year).ThenBy(g => g.Month)
                .ToListAsync();

            // Bước 2: Format chuỗi ngoài SQL
            var monthlyRevenue = monthlyRevenueRaw
                .Select(g => new MonthlyRevenueDto
                {
                    Month = $"{g.Month}/{g.Year}",
                    Revenue = g.Revenue
                })
                .ToList();

            // ✅ Recent orders: project sang DTO, ép Status -> int
            var recentOrders = await _context.Orders
                .Include(o => o.Shipping)
                .OrderByDescending(o => o.OrderDate)
                .Take(5)
                .Select(o => new RecentOrderDto
                {
                    OrderId = o.OrderId,
                    OrderDate = o.OrderDate,
                    TotalAmount = o.TotalAmount,
                    Status = (int)o.Status,                                // << quan trọng
                    RecipientName = o.Shipping != null ? o.Shipping.RecipientName : null
                })
                .ToListAsync();

            var dto = new DashboardStatsDto
            {
                TotalUsers = totalUsers,
                TotalProducts = totalProducts,
                TotalCategories = totalCategories,
                TotalOrders = totalOrders,
                TotalRevenue = totalRevenue,
                OrderStatusStats = statusStats,
                MonthlyRevenue = monthlyRevenue,
                RecentOrders = recentOrders
            };

            return Ok(new APIRespone<DashboardStatsDto>
            {
                Success = true,
                Message = "Lấy thống kê thành công",
                Data = dto
            });
        }
    }
}
