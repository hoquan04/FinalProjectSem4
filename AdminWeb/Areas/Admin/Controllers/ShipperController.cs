
using AdminWeb.Areas.Admin.Data.Services;
using AdminWeb.Areas.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ShipperController : Controller
    {
        private readonly ShipperService _shipperService;

        public ShipperController(ShipperService shipperService)
        {
            _shipperService = shipperService;
        }

        /// <summary>
        /// 📋 Hiển thị danh sách người dùng gửi yêu cầu làm Shipper
        /// </summary>
        public async Task<IActionResult> Index()
        {
            var pending = await _shipperService.GetPendingRequestsAsync();
            return View(pending);
        }

        /// <summary>
        /// ✅ Phê duyệt yêu cầu Shipper
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Approve(int userId)
        {
            var result = await _shipperService.ApproveShipperAsync(userId, true);
            TempData["Message"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// ❌ Từ chối yêu cầu Shipper
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Reject(int userId)
        {
            var result = await _shipperService.ApproveShipperAsync(userId, false);
            TempData["Message"] = result.Message;
            return RedirectToAction(nameof(Index));
        }
    }
}
