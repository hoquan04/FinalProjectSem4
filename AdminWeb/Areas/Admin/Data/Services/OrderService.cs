using AdminWeb.Areas.Admin.Data;
using AdminWeb.Areas.Admin.Models;
using AdminWeb.Areas.Admin.Models.DTOs;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AdminWeb.Areas.Admin.Data.Services
{
    public class OrderService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _options;

        public OrderService(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient("APIClient");
            _options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() }
            };
        }

        #region ====== ORDER ======

        /// <summary>
        /// 🚀 Lấy danh sách đơn hàng có phân trang (OrderDisplayDto)
        /// </summary>
        public async Task<ApiResponse<PagedResponse<OrderDisplayDto>>> GetAllOrdersAsync(int pageNow = 1, int pageSize = 10)
        {
            var url = $"{ApiConstants.OrderApi}?pageNow={pageNow}&pageSize={pageSize}";
            var response = await _httpClient.GetAsync(url);
            var raw = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"[DEBUG] API Response: {raw}");

            if (!response.IsSuccessStatusCode)
            {
                return new ApiResponse<PagedResponse<OrderDisplayDto>>
                {
                    Success = false,
                    Message = $"API lỗi {response.StatusCode}: {raw}"
                };
            }

            return JsonSerializer.Deserialize<ApiResponse<PagedResponse<OrderDisplayDto>>>(raw, _options)
                   ?? new ApiResponse<PagedResponse<OrderDisplayDto>> { Success = false, Message = "Parse lỗi JSON" };
        }

        /// <summary>
        /// 🚀 Lấy đơn hàng theo ID (Order chi tiết)
        /// </summary>
        public async Task<ApiResponse<Order>> GetOrderByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<ApiResponse<Order>>(
                $"{ApiConstants.OrderApi}/{id}", _options
            ) ?? new ApiResponse<Order>
            {
                Success = false,
                Message = "Không parse được dữ liệu từ API"
            };
        }

        /// <summary>
        /// 🚀 Tạo đơn hàng mới
        /// </summary>
        public async Task<ApiResponse<Order>> CreateOrderAsync(Order model)
        {
            var response = await _httpClient.PostAsJsonAsync($"{ApiConstants.OrderApi}", model);
            return await response.Content.ReadFromJsonAsync<ApiResponse<Order>>(_options)
                   ?? new ApiResponse<Order>
                   {
                       Success = false,
                       Message = "Không parse được dữ liệu từ API"
                   };
        }

        /// <summary>
        /// 🚀 Cập nhật đơn hàng
        /// </summary>
        public async Task<ApiResponse<Order>> UpdateOrderAsync(int id, Order model)
        {
            var response = await _httpClient.PutAsJsonAsync($"{ApiConstants.OrderApi}/{id}", model);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                return new ApiResponse<Order>
                {
                    Success = false,
                    Message = $"API lỗi {response.StatusCode}: {error}"
                };
            }

            return await response.Content.ReadFromJsonAsync<ApiResponse<Order>>(_options)
                   ?? new ApiResponse<Order>
                   {
                       Success = false,
                       Message = "Không parse được dữ liệu từ API"
                   };
        }

        /// <summary>
        /// 🚀 Xóa đơn hàng
        /// </summary>
        public async Task<ApiResponse<bool>> DeleteOrderAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{ApiConstants.OrderApi}/{id}");
            return await response.Content.ReadFromJsonAsync<ApiResponse<bool>>(_options)
                   ?? new ApiResponse<bool>
                   {
                       Success = false,
                       Message = "Không parse được dữ liệu từ API"
                   };
        }

        /// <summary>
        /// 🔍 Tìm kiếm đơn hàng (OrderDisplayDto)
        /// </summary>
        public async Task<ApiResponse<PagedResponse<OrderDisplayDto>>> SearchOrderAsync(SearchOrder search, int pageNow = 1, int pageSize = 10)
        {
            var response = await _httpClient.PostAsJsonAsync(
                $"{ApiConstants.OrderApi}/searchdto?pageNow={pageNow}&pageSize={pageSize}", search
            );

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                return new ApiResponse<PagedResponse<OrderDisplayDto>>
                {
                    Success = false,
                    Message = $"API lỗi {response.StatusCode}: {error}"
                };
            }

            return await response.Content.ReadFromJsonAsync<ApiResponse<PagedResponse<OrderDisplayDto>>>(_options)
                   ?? new ApiResponse<PagedResponse<OrderDisplayDto>>
                   {
                       Success = false,
                       Message = "Không parse được dữ liệu từ API"
                   };
        }

        #endregion
    }
}
