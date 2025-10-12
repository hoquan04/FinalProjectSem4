using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using API.Data;
using API.Helpers;
using API.Models;
using API.Models.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly DataContext _ctx;
        private readonly IConfiguration _config;

        public AuthController(DataContext ctx, IConfiguration config)
        {
            _ctx = ctx;
            _config = config;
        }

        // POST: /api/auth/login-admin
        [HttpPost("login-admin")]
        public IActionResult LoginAdmin([FromBody] SigninModel req)
        {
            var user = _ctx.Users.FirstOrDefault(u => u.Email == req.Email);

            if (user == null)
            {
                // ❌ Không dùng Unauthorized nữa
                return Ok(new { message = "Sai tài khoản hoặc mật khẩu" });
            }

            var stored = user.PasswordHash ?? string.Empty;
            var isBcrypt = stored.StartsWith("$2");
            var ok = false;

            try
            {
                if (isBcrypt)
                {
                    ok = BCrypt.Net.BCrypt.Verify(req.Password, stored);
                }
                else
                {
                    ok = req.Password == stored;
                    if (ok)
                    {
                        // nâng cấp sang bcrypt nếu còn legacy
                        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password);
                        _ctx.SaveChanges();
                    }
                }
            }
            catch
            {
                ok = false;
            }

            if (!ok)
            {
                return Ok(new { message = "Sai tài khoản hoặc mật khẩu" });
            }

            if (user.Role.ToString() != "Admin")
            {
                return Ok(new { message = "Bạn không có quyền đăng nhập vào trang quản trị" });
            }

            // phát JWT
            var jwt = _config.GetSection("Jwt");
            var token = JwtHelper.GenerateToken(
                userId: user.UserId,
                email: user.Email,
                fullName: user.FullName,
                role: user.Role.ToString(),
                key: jwt["Key"]!,
                issuer: jwt["Issuer"]!,
                audience: jwt["Audience"]!
            );

            return Ok(new
            {
                message = "Đăng nhập Admin thành công",
                token,
                user = new { user.UserId, user.FullName, user.Email, user.Role, user.CreatedAt }
            });
        }



        [HttpGet("me")]
        [Authorize] // cần JWT
        public async Task<IActionResult> Me()
        {
            var uid = User.FindFirstValue("uid"); // bạn đã nhúng 'uid' trong token
            if (string.IsNullOrEmpty(uid)) return Unauthorized();

            if (!int.TryParse(uid, out var userId)) return Unauthorized();

            var user = await _ctx.Users
                .AsNoTracking()
                .Select(u => new
                {
                    u.UserId,
                    u.FullName,
                    u.Email,
                    u.Phone,
                    u.Address,
                    Role = u.Role.ToString(),
                    u.CreatedAt
                })
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null) return NotFound(new { message = "Không tìm thấy người dùng" });
            return Ok(user);
        }



        // PUT: /api/auth/profile  -> cập nhật tên/đt/địa chỉ
        [HttpPut("profile")]
        [Authorize] // cần JWT
        public async Task<IActionResult> UpdateProfile([FromBody] ProfileUpdateRequest req)
        {
            var uid = User.FindFirstValue("uid");
            if (string.IsNullOrEmpty(uid) || !int.TryParse(uid, out var userId))
                return Unauthorized();

            var user = await _ctx.Users.FirstOrDefaultAsync(x => x.UserId == userId);
            if (user == null) return NotFound(new { message = "Không tìm thấy người dùng" });

            if (string.IsNullOrWhiteSpace(req.FullName))
                return BadRequest(new { message = "Họ tên bắt buộc" });

            user.FullName = req.FullName.Trim();
            user.Phone = req.Phone;
            user.Address = req.Address;

            await _ctx.SaveChangesAsync();

            return Ok(new
            {
                message = "Cập nhật hồ sơ thành công",
                user = new { user.UserId, user.FullName, user.Email, user.Phone, user.Address, Role = user.Role.ToString(), user.CreatedAt }
            });
        }

        // PUT: /api/auth/change-password
        [HttpPut("change-password")]
        [Authorize] // cần JWT
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.CurrentPassword) || string.IsNullOrWhiteSpace(req.NewPassword))
                return BadRequest(new { message = "Thiếu mật khẩu hiện tại hoặc mật khẩu mới" });

            if (req.NewPassword.Length < 6)
                return BadRequest(new { message = "Mật khẩu mới tối thiểu 6 ký tự" });

            var uid = User.FindFirstValue("uid");
            if (string.IsNullOrEmpty(uid) || !int.TryParse(uid, out var userId))
                return Unauthorized();

            var user = await _ctx.Users.FirstOrDefaultAsync(x => x.UserId == userId);
            if (user == null) return NotFound(new { message = "Không tìm thấy người dùng" });

            var stored = user.PasswordHash ?? string.Empty;
            var isBcrypt = stored.StartsWith("$2");
            bool ok = false;

            try
            {
                if (isBcrypt)
                {
                    ok = BCrypt.Net.BCrypt.Verify(req.CurrentPassword, stored);
                }
                else
                {
                    // legacy plaintext
                    ok = req.CurrentPassword == stored;
                }
            }
            catch { ok = false; }

            if (!ok) return Unauthorized(new { message = "Mật khẩu hiện tại không đúng" });

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.NewPassword);
            await _ctx.SaveChangesAsync();

            // ✅ Thông báo theo yêu cầu
            return Ok(new { message = "Bạn đã thay đổi mật khẩu thành công" });
        }



        [HttpPost("register")]
        public IActionResult Register([FromBody] SignUpModel req)
        {
            // Với [ApiController], ModelState invalid sẽ tự trả 400, nhưng để rõ ràng vẫn check:
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Chuẩn hoá đầu vào
            var email = req.Email?.Trim().ToLowerInvariant();
            var firstName = req.FirstName?.Trim();
            var lastName = req.LastName?.Trim();
            var phone = req.Phone?.Trim();
            var address = req.Address?.Trim();

            if (req.Password != req.ConfirmPassword)
                return BadRequest(new { message = "Password và ConfirmPassword không khớp" });

            if (_ctx.Users.Any(u => u.Email == email))
                return BadRequest(new { message = "Email đã tồn tại" });

            var user = new User
            {
                FullName = $"{firstName} {lastName}".Trim(),
                Email = email!,
                Phone = phone,
                Address = address,
                Role = UserRole.Customer,
                CreatedAt = DateTime.UtcNow,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password)
            };

            _ctx.Users.Add(user);
            _ctx.SaveChanges();

            return StatusCode(201, new
            {
                message = "Đăng ký thành công",
                user = new
                {
                    user.UserId,
                    user.FullName,
                    user.Email,
                    user.Phone,
                    user.Address,
                    Role = user.Role.ToString(),
                    user.CreatedAt
                }
            });
        }


        // POST: /api/auth/login
        [HttpPost("login")]
        public IActionResult Login([FromBody] SigninModel req)
        {
            var user = _ctx.Users.FirstOrDefault(u => u.Email == req.Email);
            if (user == null) return Ok(new { message = "Sai tài khoản hoặc mật khẩu" });

            var stored = user.PasswordHash ?? string.Empty;
            var isBcrypt = stored.StartsWith("$2");
            var ok = false;

            if (isBcrypt)
            {
                ok = BCrypt.Net.BCrypt.Verify(req.Password, stored);
            }
            else
            {
                ok = req.Password == stored;
                if (ok)
                {
                    // nâng cấp sang bcrypt
                    user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password);
                    _ctx.SaveChanges();
                }
            }

            if (!ok) return Ok(new { message = "Sai tài khoản hoặc mật khẩu" });

            // phát JWT cho user
            var jwt = _config.GetSection("Jwt");
            var token = JwtHelper.GenerateToken(
                userId: user.UserId,
                email: user.Email,
                fullName: user.FullName,
                role: user.Role.ToString(),
                key: jwt["Key"]!,
                issuer: jwt["Issuer"]!,
                audience: jwt["Audience"]!
            );

            return Ok(new
            {
                message = "Đăng nhập thành công",
                token,
                user = new { user.UserId, user.FullName, user.Email, user.Role, user.CreatedAt }
            });
        }
    }
}
