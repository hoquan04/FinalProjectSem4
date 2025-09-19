using AdminWeb.Areas.Admin.Data.Services;
using AdminWeb.Areas.Admin.Models;
using AdminWeb.Areas.Admin.Models.DTOs;
using AdminWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Globalization;

namespace AdminWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrderController : Controller
    {
        private readonly OrderService _orderService;
        private readonly OrderDetailService _orderDetailService;

        public OrderController(OrderService orderService, OrderDetailService orderDetailService)
        {
            _orderService = orderService;
            _orderDetailService = orderDetailService;
        }

        // 📌 Danh sách đơn hàng có phân trang
        public async Task<IActionResult> Index(int pageNow = 1, int pageSize = 10)
        {
            var response = await _orderService.GetOrderPageAsync(pageNow, pageSize);
            if (!response.Success) return View("Error", response.Message);

            return View(response.Data);
        }

        // 📌 Chi tiết đơn hàng
        public async Task<IActionResult> Details(int id)
        {
            var response = await _orderService.GetOrderByIdAsync(id);
            if (!response.Success) return NotFound(response.Message);

            return View(response.Data);
        }

        // 📌 Tạo mới đơn hàng (GET)
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // 📌 Tạo mới đơn hàng (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Order model)
        {
            if (!ModelState.IsValid) return View(model);

            var response = await _orderService.CreateOrderAsync(model);
            if (response.Success)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", response.Message ?? "Lỗi khi tạo đơn hàng");
            return View(model);
        }

        // 📌 Sửa đơn hàng (GET)
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var response = await _orderService.GetOrderByIdAsync(id);
            if (!response.Success || response.Data == null)
                return NotFound(response.Message);

            // Gửi danh sách trạng thái xuống view
            ViewBag.statusList = new SelectList(
                Enum.GetValues(typeof(OrderStatus))
                    .Cast<OrderStatus>()
                    .Select(s => new { Value = s, Text = s.ToString() }),
                "Value", "Text", response.Data.Status
            );

            return View(response.Data);
        }

        // POST: /Admin/Order/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Order model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.statusList = new SelectList(
                    Enum.GetValues(typeof(OrderStatus))
                        .Cast<OrderStatus>()
                        .Select(s => new { Value = s, Text = s.ToString() }),
                    "Value", "Text", model.Status
                );
                return View(model);
            }

            var response = await _orderService.UpdateOrderAsync(id, model);
            if (response.Success)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", response.Message ?? "Lỗi khi cập nhật đơn hàng");

            ViewBag.statusList = new SelectList(
                Enum.GetValues(typeof(OrderStatus))
                    .Cast<OrderStatus>()
                    .Select(s => new { Value = s, Text = s.ToString() }),
                "Value", "Text", model.Status
            );

            return View(model);
        }

        // 📌 Xóa đơn hàng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _orderService.DeleteOrderAsync(id);
            if (!response.Success) return NotFound(response.Message);

            return RedirectToAction(nameof(Index));
        }

        // 📌 Tìm kiếm đơn hàng
        // 📌 Tìm kiếm đơn hàng
        [HttpPost]
        public async Task<IActionResult> Search(SearchOrder search, int pageNow = 1, int pageSize = 10)
        {
            var rawFrom = Request.Form["FromDate"];
            var rawTo = Request.Form["ToDate"];

            if (!string.IsNullOrWhiteSpace(rawFrom))
                search.FromDate = DateTime.ParseExact(rawFrom!, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            if (!string.IsNullOrWhiteSpace(rawTo))
                search.ToDate = DateTime.ParseExact(rawTo!, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            // Bao trọn ngày ToDate
            if (search.ToDate.HasValue)
                search.ToDate = search.ToDate.Value.Date.AddDays(1).AddTicks(-1);

            var response = await _orderService.SearchOrderAsync(search, pageNow, pageSize);

            ViewBag.SearchModel = search; // giữ lại dữ liệu tìm kiếm
            return View("Index", response.Data);
        }

        public async Task<IActionResult> OrderDetail(int orderId, int pageNow = 1, int pageSize = 10)
        {
            var response = await _orderDetailService.GetByOrderIdAsync(orderId, pageNow, pageSize);
            if (!response.Success || response.Data == null)
                return View("Error", response.Message);

            ViewBag.OrderId = orderId;
            return View(response.Data); // Model = PagedResponse<OrderDetailDto>
        }


    }
}
