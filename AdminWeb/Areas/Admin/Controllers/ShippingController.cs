using AdminWeb.Areas.Admin.Data.Services;
using AdminWeb.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc;

namespace AdminWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ShippingController : Controller
    {
        private readonly ShippingApiService _shippingApiService;

        public ShippingController(ShippingApiService shippingApiService)
        {
            _shippingApiService = shippingApiService;
        }
        public async Task<IActionResult> Index(string? searchString)
        {
            ViewBag.SearchString = searchString;

            try
            {
                List<Shipping> shippings;

                // Gọi API thông qua ShippingService
                if (!string.IsNullOrEmpty(searchString))
                {
                    var searchModel = new ShippingSearchModel { SearchTerm = searchString };
                    shippings = await _shippingApiService.SearchShippingAsync(searchModel);
                }
                else
                {
                    shippings = await _shippingApiService.GetAllShippingAsync();
                }

                return View(shippings);
            }
            catch (Exception ex)
            {
                // Error handling
                ViewBag.Error = $"Lỗi khi tải danh sách shipping: {ex.Message}";
                ViewBag.ErrorDetail = "Vui lòng kiểm tra API đã chạy chưa hoặc kết nối mạng.";
                return View(new List<Shipping>());
            }
        }

        public IActionResult Create()
        {
            return View(new ShippingCreateModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ShippingCreateModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // Gọi API POST thông qua CategoryService
                var result = await _shippingApiService.CreateShippingAsync(model);

                if (result.Success)
                {
                    TempData["SuccessMessage"] = result.Message ?? "Thêm shipping thành công!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", result.Message ?? "Có lỗi xảy ra khi thêm shipping");
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Lỗi hệ thống: {ex.Message}");
                return View(model);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                // Gọi API GET by ID thông qua CategoryService
                var shipping = await _shippingApiService.GetShippingByIdAsync(id);
                if (shipping == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy shipping";
                    return RedirectToAction(nameof(Index));
                }

                // Convert sang EditModel
                var editModel = new ShippingEditModel
                {
                    ShippingId = shipping.ShippingId,
                    Address = shipping.Address,
                    City = shipping.City,
                    PostalCode = shipping.PostalCode,
                    ShippingFee = shipping.ShippingFee,
                    EstimatedDays = shipping.EstimatedDays
                };

                return View(editModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi khi tải shipping: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ShippingEditModel model)
        {
            if (id != model.ShippingId)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // Gọi API PUT thông qua CategoryService
                var result = await _shippingApiService.UpdateShippingAsync(id, model);
                
                if (result.Success)
                {
                    TempData["SuccessMessage"] = result.Message ?? "Cập nhật shipping thành công!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", result.Message ?? "Có lỗi xảy ra khi cập nhật shipping");
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Lỗi hệ thống: {ex.Message}");
                return View(model);
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var shipping = await _shippingApiService.GetShippingByIdAsync(id);
                if (shipping == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy shipping";
                    return RedirectToAction(nameof(Index));
                }
                return View(shipping);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi khi tải shipping: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var result = await _shippingApiService.DeleteShippingAsync(id);

                if (result.Success)
                {
                    TempData["SuccessMessage"] = result.Message ?? "Xóa shipping thành công!";
                }
                else
                {
                    TempData["ErrorMessage"] = result.Message ?? "Có lỗi xảy ra khi xóa shipping";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi hệ thống: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
