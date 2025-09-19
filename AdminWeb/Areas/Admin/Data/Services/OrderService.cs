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
        /// 🚀 Lấy tất cả đơn hàng
        /// </summary>
        public async Task<ApiResponse<IEnumerable<Order>>> GetAllOrdersAsync()
        {
            return await _httpClient.GetFromJsonAsync<ApiResponse<IEnumerable<Order>>>(
                $"{ApiConstants.OrderApi}", _options
            ) ?? new ApiResponse<IEnumerable<Order>>
            {
                Success = false,
                Message = "Không parse được dữ liệu từ API"
            };
        }

        /// <summary>
        /// 🚀 Lấy đơn hàng theo ID
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
        /// 🚀 Lấy danh sách đơn hàng có phân trang
        /// </summary>
        public async Task<ApiResponse<PagedResponse<Order>>> GetOrderPageAsync(int pageNow = 1, int pageSize = 10)
        {
            return await _httpClient.GetFromJsonAsync<ApiResponse<PagedResponse<Order>>>(
                $"{ApiConstants.OrderApi}/page?pageNow={pageNow}&pageSize={pageSize}", _options
            ) ?? new ApiResponse<PagedResponse<Order>>
            {
                Success = false,
                Message = "Không parse được dữ liệu từ API"
            };
        }

        /// <summary>
        /// 🔍 Tìm kiếm đơn hàng
        /// </summary>
        public async Task<ApiResponse<PagedResponse<Order>>> SearchOrderAsync(SearchOrder search, int pageNow = 1, int pageSize = 10)
        {
            var response = await _httpClient.PostAsJsonAsync(
                $"{ApiConstants.OrderApi}/search?pageNow={pageNow}&pageSize={pageSize}", search
            );

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                return new ApiResponse<PagedResponse<Order>>
                {
                    Success = false,
                    Message = $"API lỗi {response.StatusCode}: {error}"
                };
            }

            return await response.Content.ReadFromJsonAsync<ApiResponse<PagedResponse<Order>>>(_options)
                   ?? new ApiResponse<PagedResponse<Order>>
                   {
                       Success = false,
                       Message = "Không parse được dữ liệu từ API"
                   };
        }

        #endregion
    }
}
