using AdminWeb.Areas.Admin.Data.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly DashboardService _dashboardService;

        public DashboardController(DashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        public async Task<IActionResult> Index()
        {
            var result = await _dashboardService.GetDashboardAsync();
            if (!result.Success || result.Data == null)
            {
                ViewBag.Error = result.Message;
                return View(new DashboardStatsViewModel());
            }

            return View(result.Data);
        }
    }
}
