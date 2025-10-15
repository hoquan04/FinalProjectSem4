using AdminWeb.Areas.Admin.Data.Services;
using AdminWeb.Areas.Admin.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AdminWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountController : Controller
    {
        private readonly AuthService _auth;
        private readonly TokenValidationParameters _tokenParams;

        public AccountController(AuthService auth, TokenValidationParameters tokenParams)
        {
            _auth = auth;
            _tokenParams = tokenParams;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (!ModelState.IsValid) return View(model);

            var (ok, token, error) = await _auth.LoginAsync(model.Email, model.Password);
            if (!ok || string.IsNullOrEmpty(token))
            {
                // 🚨 Hiển thị thông báo lỗi từ API
                ModelState.AddModelError("", error ?? "Đăng nhập thất bại");
                return View(model);
            }

            var handler = new JwtSecurityTokenHandler();
            var principal = handler.ValidateToken(token, _tokenParams, out var _);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(principal.Identity as ClaimsIdentity),
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
                });

            HttpContext.Session.SetString("JWT_TOKEN", token);

            TempData["SuccessMessage"] = "Đăng nhập thành công";

            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Remove("JWT_TOKEN");
            return RedirectToAction("Login");
        }

        public IActionResult AccessDenied() => Content("Access Denied");
    }
}
