using System.Text;
using System.Text.Json;
using AdminWeb.Areas.Admin.Models;

namespace AdminWeb.Areas.Admin.Data.Services
{
    public class AuthService
    {
        private readonly HttpClient _http;
        private readonly IHttpContextAccessor _ctx;
        private readonly JsonSerializerOptions _json = new() { PropertyNameCaseInsensitive = true };

        public AuthService(HttpClient http, IHttpContextAccessor ctx)
        {
            _http = http;
            _ctx = ctx;
        }

        public async Task<(bool ok, string? token, string? error)> LoginAsync(string email, string password)
        {
            var body = JsonSerializer.Serialize(new { email, password });
            var resp = await _http.PostAsync("auth/login-admin",
                new StringContent(body, Encoding.UTF8, "application/json"));

            var json = await resp.Content.ReadAsStringAsync();

            // parse JSON để lấy message
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            // Nếu không có token => coi như lỗi
            if (!root.TryGetProperty("token", out var tokenEl))
            {
                var msg = root.TryGetProperty("message", out var m) ? m.GetString() : "Đăng nhập thất bại";
                return (false, null, msg);
            }

            var token = tokenEl.GetString();
            if (string.IsNullOrWhiteSpace(token))
            {
                var msg = root.TryGetProperty("message", out var m) ? m.GetString() : "Đăng nhập thất bại";
                return (false, null, msg);
            }

            return (true, token, null);
        }

        public void Logout()
        {
            _ctx.HttpContext?.Session.Remove("JWT_TOKEN");
            _ctx.HttpContext?.Session.Remove("CurrentUserJson");
        }
    }
}
