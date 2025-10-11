using AdminWeb.Areas.Admin.Data.Services;
using AdminWeb.Areas.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        // GET: /Admin/User?searchString=...&page=1&pageSize=10
        public async Task<IActionResult> Index(string? searchString, int page = 1, int pageSize = 10)
        {
            var resp = await _userService.GetUserPageAsync(searchString, page, pageSize);
            if (!resp.Success)
            {
                ViewBag.Error = resp.Message ?? "Không tải được danh sách người dùng";
                ViewBag.SearchString = searchString;
                return View(new PagedResponse<UserViewModel> { PageNow = page, PageSize = pageSize, TotalCount = new() });
            }

            ViewBag.SearchString = searchString; // giữ từ khoá tìm kiếm trong ô input
            return View(resp.Data);
        }

        // Các action Create/Edit/Delete của bạn giữ nguyên
        public IActionResult Create() => View(new UserCreateModel());

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserCreateModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var result = await _userService.CreateUserAsync(model);
            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message ?? "Thêm người dùng thành công!";
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError("", result.Message ?? "Có lỗi xảy ra khi thêm người dùng");
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy người dùng";
                return RedirectToAction(nameof(Index));
            }
            var editModel = new UserEditModel
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.Phone,
                Address = user.Address,
                Role = user.Role
            };
            return View(editModel);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserEditModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var result = await _userService.UpdateUserAsync(model.UserId, model);
            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message ?? "Cập nhật thành công!";
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError("", result.Message ?? "Có lỗi xảy ra khi cập nhật");
            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy người dùng";
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _userService.DeleteUserAsync(id);
            if (result.Success)
                TempData["SuccessMessage"] = result.Message ?? "Xóa thành công!";
            else
                TempData["ErrorMessage"] = result.Message ?? "Có lỗi xảy ra khi xóa";
            return RedirectToAction(nameof(Index));
        }
    }
}
