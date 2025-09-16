using System.Text;
using System.Text.Json;
using AdminWeb.Areas.Admin.Models;

namespace AdminWeb.Areas.Admin.Data.Services
{
    public class ProfileService
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _json = new() { PropertyNameCaseInsensitive = true };

        public ProfileService(HttpClient http) => _http = http;

        public async Task<AuthUser?> GetMeAsync()
        {
            var resp = await _http.GetAsync("auth/me");
            if (!resp.IsSuccessStatusCode) return null;

            var body = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<AuthUser>(body, _json);
        }

        public async Task<(bool ok, string msg)> UpdateProfileAsync(string fullName, string? phone, string? address)
        {
            var payload = JsonSerializer.Serialize(new { fullName, phone, address });
            var resp = await _http.PutAsync("auth/profile",
                new StringContent(payload, Encoding.UTF8, "application/json"));
            var body = await resp.Content.ReadAsStringAsync();

            if (!resp.IsSuccessStatusCode)
                return (false, string.IsNullOrWhiteSpace(body) ? "Cập nhật thất bại" : body);

            try
            {
                using var doc = JsonDocument.Parse(body);
                var msg = doc.RootElement.TryGetProperty("message", out var m) ? m.GetString() : "Thành công";
                return (true, msg ?? "Thành công");
            }
            catch { return (true, "Cập nhật thành công"); }
        }

        public async Task<(bool ok, string msg)> ChangePasswordAsync(string currentPassword, string newPassword)
        {
            var payload = JsonSerializer.Serialize(new { currentPassword, newPassword });
            var resp = await _http.PutAsync("auth/change-password",
                new StringContent(payload, Encoding.UTF8, "application/json"));
            var body = await resp.Content.ReadAsStringAsync();

            if (!resp.IsSuccessStatusCode)
                return (false, string.IsNullOrWhiteSpace(body) ? "Đổi mật khẩu thất bại" : body);

            try
            {
                using var doc = JsonDocument.Parse(body);
                var msg = doc.RootElement.TryGetProperty("message", out var m) ? m.GetString() : "Thành công";
                return (true, msg ?? "Thành công");
            }
            catch { return (true, "Bạn đã thay đổi mật khẩu thành công"); }
        }
    }
}
