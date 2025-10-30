using AdminWeb.Areas.Admin.Data.Services;
using AdminWeb.Areas.Admin.Models;
using AdminWeb.Areas.Admin.Models.DTOs;
using AdminWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Globalization;

namespace AdminWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
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
            var response = await _orderService.GetAllOrdersAsync(pageNow, pageSize);

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

        //        // 📌 Sửa đơn hàng (GET)
        //        [HttpGet]
        //        public async Task<IActionResult> Edit(int id)
        //        {
        //            var response = await _orderService.GetOrderByIdAsync(id);
        //            if (!response.Success || response.Data == null)
        //                return NotFound(response.Message);

        //            var order = response.Data;

        //            // Lấy User
        //            var userService = HttpContext.RequestServices.GetRequiredService<UserService>();
        //            var user = await userService.GetUserByIdAsync(order.UserId);

        //            // Lấy Shipping
        //            var shippingService = HttpContext.RequestServices.GetRequiredService<ShippingApiService>();
        //            var shipping = await shippingService.GetShippingByIdAsync(order.ShippingId);

        //            // Gửi dữ liệu xuống View
        //            ViewBag.UserAndRecipient = $"{(user?.FullName ?? $"User #{order.UserId}")} - {(shipping?.RecipientName ?? $"Recipient #{order.ShippingId}")}";
        //            ViewBag.ShippingAddress = shipping?.Address ?? "";


        //            // ✅ Chỉ cho phép chọn "Xác nhận" và "Hủy"
        //            var allowedStatuses = new[]
        //            {
        //                OrderStatus.Confirmed, // Xác nhận
        //                OrderStatus.Cancelled  // Hủy
        //};

        //            ViewBag.statusList = new SelectList(
        //                allowedStatuses.Select(s => new
        //                {
        //                    Value = s,
        //                    Text = s == OrderStatus.Confirmed ? "Xác nhận" :
        //                           s == OrderStatus.Cancelled ? "Hủy" : s.ToString()
        //                }),
        //                "Value", "Text", order.Status
        //            );


        //            return View(order);
        //        }


        //        // POST: /Admin/Order/Edit/5
        //        [HttpPost]
        //        [ValidateAntiForgeryToken]
        //        public async Task<IActionResult> Edit(int id, Order model)
        //        {
        //            if (!ModelState.IsValid)
        //            {
        //                ViewBag.statusList = new SelectList(
        //                    Enum.GetValues(typeof(OrderStatus))
        //                        .Cast<OrderStatus>()
        //                        .Select(s => new { Value = s, Text = s.ToString() }),
        //                    "Value", "Text", model.Status
        //                );
        //                return View(model);
        //            }

        //            var response = await _orderService.UpdateOrderAsync(id, model);
        //            if (response.Success)
        //                return RedirectToAction(nameof(Index));

        //            ModelState.AddModelError("", response.Message ?? "Lỗi khi cập nhật đơn hàng");

        //            ViewBag.statusList = new SelectList(
        //                Enum.GetValues(typeof(OrderStatus))
        //                    .Cast<OrderStatus>()
        //                    .Select(s => new { Value = s, Text = s.ToString() }),
        //                "Value", "Text", model.Status
        //            );

        //            return View(model);
        //        }
        // 📌 Sửa đơn hàng (GET)
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var response = await _orderService.GetOrderByIdAsync(id);
            if (!response.Success || response.Data == null)
                return NotFound(response.Message);

            var order = response.Data;

            // Lấy thông tin user
            var userService = HttpContext.RequestServices.GetRequiredService<UserService>();
            var user = await userService.GetUserByIdAsync(order.UserId);

            // Lấy thông tin shipping
            var shippingService = HttpContext.RequestServices.GetRequiredService<ShippingApiService>();
            var shipping = await shippingService.GetShippingByIdAsync(order.ShippingId);

            // ✅ Hiển thị thông tin phụ trong View
            ViewBag.UserAndRecipient =
                $"{(user?.FullName ?? $"User #{order.UserId}")} - {(shipping?.RecipientName ?? $"Recipient #{order.ShippingId}")}";
            ViewBag.ShippingAddress = shipping?.Address ?? "";

            // ✅ Chỉ cho phép chọn "Xác nhận" và "Hủy"
            var allowedStatuses = new List<SelectListItem>
    {
        new SelectListItem { Value = OrderStatus.Confirmed.ToString(), Text = "Xác nhận" },
        new SelectListItem { Value = OrderStatus.Cancelled.ToString(), Text = "Hủy" }
    };

            // ✅ Nếu trạng thái hiện tại không nằm trong 2 cái trên → thêm vào đầu danh sách
            if (!allowedStatuses.Any(s => s.Value == order.Status.ToString()))
            {
                allowedStatuses.Insert(0, new SelectListItem
                {
                    Value = order.Status.ToString(),
                    Text = order.Status switch
                    {
                        OrderStatus.Pending => "Chờ xử lý",
                        OrderStatus.Shipping => "Đang giao hàng",
                        OrderStatus.Completed => "Hoàn tất",
                        _ => order.Status.ToString()
                    },
                    Selected = true
                });
            }

            ViewBag.statusList = allowedStatuses;

            return View(order);
        }


        // 📌 Sửa đơn hàng (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Order model)
        {
            if (!ModelState.IsValid)
            {
                await RebuildStatusListAsync(model.Status);
                return View(model);
            }

            // ✅ Lấy trạng thái hiện tại của đơn hàng trong DB
            var current = await _orderService.GetOrderByIdAsync(id);
            if (current.Data == null)
            {
                TempData["Error"] = "Không tìm thấy đơn hàng.";
                return RedirectToAction(nameof(Index));
            }

            var oldStatus = current.Data.Status;
            var newStatus = model.Status;

            // ✅ Nếu trạng thái không thay đổi → chỉ quay lại mà không thông báo
            if (oldStatus == newStatus)
                return RedirectToAction(nameof(Index));

            // ❌ Nếu từ "Hủy" → "Hoàn tất" thì không cho phép
            if (oldStatus == OrderStatus.Cancelled && newStatus == OrderStatus.Completed)
            {
                TempData["Error"] = "Không thể chuyển đơn hàng đã bị hủy sang hoàn tất.";
                return RedirectToAction(nameof(Index));
            }

            // ❌ Nếu từ "Đang giao hàng" → "Xác nhận" thì không cho phép
            if (oldStatus == OrderStatus.Shipping && newStatus == OrderStatus.Confirmed)
            {
                TempData["Error"] = "Không thể chuyển đơn hàng đang giao sang xác nhận.";
                return RedirectToAction(nameof(Index));
            }

            // ❌ Nếu trạng thái cũ là "Hoàn tất" → không cho đổi nữa (kể cả sang Hủy)
            if (oldStatus == OrderStatus.Completed)
            {
                TempData["Error"] = "Đơn hàng đã hoàn tất, không thể thay đổi trạng thái.";
                return RedirectToAction(nameof(Index));
            }

            // ❌ Nếu trạng thái cũ là "Hủy" hoặc "Xác nhận" → không cho đổi nữa (trừ trường hợp giữ nguyên)
            if (oldStatus == OrderStatus.Cancelled || oldStatus == OrderStatus.Confirmed)
            {
                TempData["Error"] = "Không thể thay đổi trạng thái của đơn hàng đã hủy hoặc đã xác nhận.";
                return RedirectToAction(nameof(Index));
            }

            // ✅ Gọi API cập nhật
            var response = await _orderService.UpdateOrderAsync(id, model);
            if (response.Success)
            {
                TempData["Success"] = "Cập nhật trạng thái đơn hàng thành công!";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", response.Message ?? "Lỗi khi cập nhật đơn hàng");
            await RebuildStatusListAsync(model.Status);
            return View(model);
        }



        // 🧩 Helper method để tái tạo danh sách trạng thái nếu cần reload lại form
        private async Task RebuildStatusListAsync(OrderStatus currentStatus)
        {
            var allowedStatuses = new List<SelectListItem>
    {
        new SelectListItem { Value = OrderStatus.Confirmed.ToString(), Text = "Xác nhận" },
        new SelectListItem { Value = OrderStatus.Cancelled.ToString(), Text = "Hủy" }
    };

            if (!allowedStatuses.Any(s => s.Value == currentStatus.ToString()))
            {
                allowedStatuses.Insert(0, new SelectListItem
                {
                    Value = currentStatus.ToString(),
                    Text = currentStatus switch
                    {
                        OrderStatus.Pending => "Chờ xử lý",
                        OrderStatus.Shipping => "Đang giao hàng",
                        OrderStatus.Completed => "Hoàn tất",
                        _ => currentStatus.ToString()
                    },
                    Selected = true
                });
            }

            ViewBag.statusList = allowedStatuses;
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
