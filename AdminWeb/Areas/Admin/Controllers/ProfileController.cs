    using AdminWeb.Areas.Admin.Data.Services;
    using AdminWeb.Areas.Admin.Models;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    namespace AdminWeb.Areas.Admin.Controllers
    {
        [Area("Admin")]
        [Authorize(Roles = "Admin")]
        public class ProfileController : Controller
        {
            private readonly ProfileService _profile;

            public ProfileController(ProfileService profile)
            {
                _profile = profile;
            }

            // GET: /Admin/Profile
            public async Task<IActionResult> Index()
            {
                var me = await _profile.GetMeAsync();
                if (me == null)
                {
                    TempData["ErrorMessage"] = "Không tải được hồ sơ.";
                    return RedirectToAction("Login", "Account");
                }

                var vm = new ProfileEditViewModel
                {
                    FullName = me.FullName,
                    Email = me.Email,
                    Phone = me.Phone,
                    Address = me.Address,
                    CreatedAt = me.CreatedAt
                };

                return View(vm);
            }

            [HttpPost, ValidateAntiForgeryToken]
            public async Task<IActionResult> Index(ProfileEditViewModel model)
            {
                if (!ModelState.IsValid) return View(model);

                var (ok, msg) = await _profile.UpdateProfileAsync(model.FullName, model.Phone, model.Address);
                if (ok)
                {
                    TempData["SuccessMessage"] = msg;
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError("", msg);
                return View(model);
            }

            // GET: /Admin/Profile/ChangePassword
            public IActionResult ChangePassword() => View(new ChangePasswordViewModel());

            [HttpPost, ValidateAntiForgeryToken]
            public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
            {
                if (!ModelState.IsValid) return View(model);
                if (model.NewPassword != model.ConfirmNewPassword)
                {
                    ModelState.AddModelError("", "Xác nhận mật khẩu mới không khớp");
                    return View(model);
                }

                var (ok, msg) = await _profile.ChangePasswordAsync(model.CurrentPassword, model.NewPassword);
                if (ok)
                {
                    TempData["SuccessMessage"] = msg;
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError("", msg);
                return View(model);
            }
        }
    }
