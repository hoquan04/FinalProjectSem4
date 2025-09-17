using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using AdminWeb.Areas.Admin.Models;

namespace AdminWeb.Areas.Admin.Data.Services
{
    public class UserService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };
        // Base API url (khai báo ở ApiConstants hoặc appsettings)
        private readonly string _userApi = ApiConstants.UserApi; // ví dụ "http://localhost:7245/api/user"

        public UserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            // nếu muốn set base address: _httpClient.BaseAddress = new Uri(ApiConstants.BaseApiUrl);
        }

        // Lấy toàn bộ users (trả về List<UserViewModel>)
        public async Task<List<UserViewModel>> GetAllUsersAsync()
        {
            var resp = await _httpClient.GetAsync(_userApi);
            if (!resp.IsSuccessStatusCode) return new List<UserViewModel>();

            var body = await resp.Content.ReadAsStringAsync();
            var apiResp = JsonSerializer.Deserialize<ApiResponse<List<UserViewModel>>>(body, _jsonOptions);
            return apiResp?.Data ?? new List<UserViewModel>();
        }

        // Lấy theo id (trả về UserViewModel? để controller check null)
        public async Task<UserViewModel?> GetUserByIdAsync(int id)
        {
            var resp = await _httpClient.GetAsync($"{_userApi}/{id}");
            if (!resp.IsSuccessStatusCode) return null;

            var body = await resp.Content.ReadAsStringAsync();
            var apiResp = JsonSerializer.Deserialize<ApiResponse<UserViewModel>>(body, _jsonOptions);
            return apiResp?.Data;
        }

        // Tìm kiếm (trả List<UserViewModel>)
        public async Task<List<UserViewModel>> SearchUsersAsync(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword)) return new List<UserViewModel>();

            // Sửa endpoint đúng với API
            var resp = await _httpClient.GetAsync($"{_userApi}/search?keyword={Uri.EscapeDataString(keyword)}");

            if (!resp.IsSuccessStatusCode) return new List<UserViewModel>();

            var body = await resp.Content.ReadAsStringAsync();
            var apiResp = JsonSerializer.Deserialize<ApiResponse<List<UserViewModel>>>(body, _jsonOptions);
            return apiResp?.Data ?? new List<UserViewModel>();
        }

        // Tạo user (trả ApiResponse<UserViewModel> để controller kiểm tra Success)
        public async Task<ApiResponse<UserViewModel>> CreateUserAsync(UserCreateModel model)
        {
            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var resp = await _httpClient.PostAsync(_userApi, content);
            var body = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ApiResponse<UserViewModel>>(body, _jsonOptions)
                   ?? new ApiResponse<UserViewModel> { Success = false, Message = "Không nhận được phản hồi" };
        }

        // Cập nhật user
        public async Task<ApiResponse<UserViewModel>> UpdateUserAsync(int id, UserEditModel model)
        {
            try
            {
                var json = JsonSerializer.Serialize(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var resp = await _httpClient.PutAsync($"{_userApi}/{id}", content);

                if (!resp.IsSuccessStatusCode)
                {
                    var msg = await resp.Content.ReadAsStringAsync();
                    return new ApiResponse<UserViewModel>
                    {
                        Success = false,
                        Message = !string.IsNullOrEmpty(msg) ? msg : "Lỗi khi cập nhật user"
                    };
                }

                var body = await resp.Content.ReadAsStringAsync();
                var apiResp = JsonSerializer.Deserialize<ApiResponse<UserViewModel>>(body, _jsonOptions);
                return apiResp ?? new ApiResponse<UserViewModel> { Success = false, Message = "Không nhận được phản hồi" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<UserViewModel> { Success = false, Message = ex.Message };
            }
        }

        // Xóa user (trả ApiResponse<bool>)
        public async Task<ApiResponse<bool>> DeleteUserAsync(int id)
        {
            var resp = await _httpClient.DeleteAsync($"{_userApi}/{id}");
            var body = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ApiResponse<bool>>(body, _jsonOptions)
                   ?? new ApiResponse<bool> { Success = false, Message = "Không nhận được phản hồi" };
        }
    }
}
