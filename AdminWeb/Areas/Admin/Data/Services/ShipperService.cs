using System.Net.Http;
using System.Text;
using System.Text.Json;
using AdminWeb.Areas.Admin.Models;

namespace AdminWeb.Areas.Admin.Data.Services
{
    public class ShipperService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _options = new() { PropertyNameCaseInsensitive = true };
        private readonly string _api = ApiConstants.ShipperApi; // ví dụ "http://localhost:7245/api/shipper"

        public ShipperService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // 🧩 Gửi yêu cầu đăng ký Shipper (người dùng)
        public async Task<ApiResponse<UserViewModel>> RequestShipperAsync(int userId, string frontUrl, string backUrl)
        {
            var payload = new
            {
                UserId = userId,
                CccdFrontUrl = frontUrl,
                CccdBackUrl = backUrl
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var resp = await _httpClient.PostAsync($"{_api}/request", content);
            var body = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ApiResponse<UserViewModel>>(body, _options)
                   ?? new ApiResponse<UserViewModel> { Success = false, Message = "Không nhận được phản hồi" };
        }

        // ✅ (Admin) Duyệt hoặc từ chối yêu cầu Shipper
        public async Task<ApiResponse<UserViewModel>> ApproveShipperAsync(int userId, bool isApproved)
        {
            var resp = await _httpClient.PutAsync($"{_api}/approve?userId={userId}&isApproved={isApproved}", null);
            var body = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ApiResponse<UserViewModel>>(body, _options)
                   ?? new ApiResponse<UserViewModel> { Success = false, Message = "Không nhận được phản hồi" };
        }

        // 📋 Lấy danh sách tất cả user có yêu cầu Shipper
        public async Task<List<UserViewModel>> GetPendingRequestsAsync()
        {
            var resp = await _httpClient.GetAsync($"{_api}/pending");
            if (!resp.IsSuccessStatusCode) return new List<UserViewModel>();

            var body = await resp.Content.ReadAsStringAsync();
            var apiResp = JsonSerializer.Deserialize<ApiResponse<List<UserViewModel>>>(body, _options);
            return apiResp?.Data ?? new List<UserViewModel>();
        }
    }
}
