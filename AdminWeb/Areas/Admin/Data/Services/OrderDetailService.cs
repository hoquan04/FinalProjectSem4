using AdminWeb.Areas.Admin.Models;
using AdminWeb.Areas.Admin.Models.DTOs;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AdminWeb.Areas.Admin.Data.Services
{
    public class OrderDetailService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _options;

        public OrderDetailService(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient("APIClient");
            _options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() }
            };
        }

        // 🚀 Lấy danh sách chi tiết đơn hàng theo OrderId
        public async Task<ApiResponse<PagedResponse<OrderDetailDto>>> GetByOrderIdAsync(int orderId, int pageNow = 1, int pageSize = 10)
        {
            var search = new SearchOrderDetail { OrderId = orderId };

            var response = await _httpClient.PostAsJsonAsync(
                $"{ApiConstants.OrderDetailApi}/search?pageNow={pageNow}&pageSize={pageSize}",
                search
            );

            return await response.Content.ReadFromJsonAsync<ApiResponse<PagedResponse<OrderDetailDto>>>(_options)
                   ?? new ApiResponse<PagedResponse<OrderDetailDto>>
                   {
                       Success = false,
                       Message = "Không parse được dữ liệu từ API"
                   };
        }

        // 🚀 Lấy danh sách chi tiết đơn hàng có phân trang
        public async Task<ApiResponse<PagedResponse<OrderDetailDto>>> GetOrderDetailPageAsync(int pageNow = 1, int pageSize = 10)
        {
            return await _httpClient.GetFromJsonAsync<ApiResponse<PagedResponse<OrderDetailDto>>>(
                $"{ApiConstants.OrderDetailApi}/paged?pageNow={pageNow}&pageSize={pageSize}", _options
            ) ?? new ApiResponse<PagedResponse<OrderDetailDto>>
            {
                Success = false,
                Message = "Không parse được dữ liệu từ API"
            };
        }

        // 🚀 Lấy tất cả chi tiết đơn hàng
        public async Task<ApiResponse<IEnumerable<OrderDetailDto>>> GetAllOrderDetailsAsync()
        {
            return await _httpClient.GetFromJsonAsync<ApiResponse<IEnumerable<OrderDetailDto>>>(
                $"{ApiConstants.OrderDetailApi}", _options
            ) ?? new ApiResponse<IEnumerable<OrderDetailDto>>
            {
                Success = false,
                Message = "Không parse được dữ liệu từ API"
            };
        }

        // 🚀 Lấy chi tiết đơn hàng theo ID
        public async Task<ApiResponse<OrderDetailDto>> GetOrderDetailByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<ApiResponse<OrderDetailDto>>(
                $"{ApiConstants.OrderDetailApi}/{id}", _options
            ) ?? new ApiResponse<OrderDetailDto>
            {
                Success = false,
                Message = "Không parse được dữ liệu từ API"
            };
        }

        // 🚀 Tạo chi tiết đơn hàng
        public async Task<ApiResponse<OrderDetailDto>> CreateOrderDetailAsync(OrderDetailDto model)
        {
            var response = await _httpClient.PostAsJsonAsync($"{ApiConstants.OrderDetailApi}", model);
            return await response.Content.ReadFromJsonAsync<ApiResponse<OrderDetailDto>>(_options)
                   ?? new ApiResponse<OrderDetailDto>
                   {
                       Success = false,
                       Message = "Không parse được dữ liệu từ API"
                   };
        }

        // 🚀 Cập nhật chi tiết đơn hàng
        public async Task<ApiResponse<OrderDetailDto>> UpdateOrderDetailAsync(int id, OrderDetailDto model)
        {
            var response = await _httpClient.PutAsJsonAsync($"{ApiConstants.OrderDetailApi}/{id}", model);
            return await response.Content.ReadFromJsonAsync<ApiResponse<OrderDetailDto>>(_options)
                   ?? new ApiResponse<OrderDetailDto>
                   {
                       Success = false,
                       Message = "Không parse được dữ liệu từ API"
                   };
        }

        // 🚀 Xóa chi tiết đơn hàng
        public async Task<ApiResponse<bool>> DeleteOrderDetailAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{ApiConstants.OrderDetailApi}/{id}");
            return await response.Content.ReadFromJsonAsync<ApiResponse<bool>>(_options)
                   ?? new ApiResponse<bool>
                   {
                       Success = false,
                       Message = "Không parse được dữ liệu từ API"
                   };
        }
    }
}
