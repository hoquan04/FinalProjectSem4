using AdminWeb.Areas.Admin.Data.Services;
using AdminWeb.Areas.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AdminWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class PaymentController : Controller
    {
        private readonly PaymentService _paymentService;

        public PaymentController(PaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        public async Task<IActionResult> Index(
    int? paymentId,
    PaymentStatus? status,
    int page = 1,
    int pageSize = 10)
        {
            ViewBag.PaymentId = paymentId;
            ViewBag.Status = status;
            ViewBag.PageNow = page;
            ViewBag.PageSize = pageSize;

            try
            {
                var payments = await _paymentService.GetPaymentsAsync(
                    searchPaymentId: paymentId,
                    status: status
                );

                var totalRecords = payments.Count;
                var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
                var items = payments
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                ViewBag.TotalPages = totalPages;
                ViewBag.TotalRecords = totalRecords;

                return View(items);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Lỗi khi tải danh sách payment: {ex.Message}";
                return View(new List<Payment>());
            }
        }


        // Xem chi tiết payment
        public async Task<IActionResult> Details(int id)
        {
            var payment = await _paymentService.GetPaymentByIdAsync(id);
            if (payment == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy payment";
                return RedirectToAction(nameof(Index));
            }

            return View(payment);
        }
    }
}
