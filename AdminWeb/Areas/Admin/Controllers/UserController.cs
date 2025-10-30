using AdminWeb.Areas.Admin.Data.Services;
using AdminWeb.Areas.Admin.Filters;
using AdminWeb.Areas.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

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

        // GET: danh sách
        public async Task<IActionResult> Index(string? searchString, int page = 1, int pageSize = 10)
        {
            ViewBag.SearchString = searchString;

            try
            {
                var res = await _userService.GetUserPageAsync(searchString, page, pageSize);
                if (res.Success && res.Data != null)
                {
                    // res.Data là PagedResponse<UserViewModel> với Data là List<UserViewModel>
                    return View(res.Data);
                }

                ViewBag.Error = res.Message ?? "Không lấy được dữ liệu";
                return View(new PagedResponse<UserViewModel>
                {
                    PageNow = page,
                    PageSize = pageSize,
                    Data = new List<UserViewModel>(),
                    TotalCount = 0,
                    TotalPage = 0
                });
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Lỗi khi tải danh sách người dùng: {ex.Message}";
                return View(new PagedResponse<UserViewModel>
                {
                    PageNow = page,
                    PageSize = pageSize,
                    Data = new List<UserViewModel>(),
                    TotalCount = 0,
                    TotalPage = 0
                });
            }
        }


        // GET: tạo mới
        public IActionResult Create() => View(new UserCreateModel());

        // POST: tạo mới
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

            // ✅ Dropdown Role
            ViewBag.RoleList = new SelectList(Enum.GetValues(typeof(UserRole)));

            return View(editModel);
        }


        // POST: chỉnh sửa
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


        // GET: xác nhận xóa
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

        // POST: xóa
        [HttpPost, ValidateAntiForgeryToken]
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
