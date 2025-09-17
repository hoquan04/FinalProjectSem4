using AdminWeb.Areas.Admin.Data;
using AdminWeb.Areas.Admin.Models;
using AdminWeb.Areas.Admin.Models.DTOs;
using System.Net.Http.Json;
using System.Text.Json;

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
                PropertyNameCaseInsensitive = true
            };
        }

        /// <summary>
        /// 🚀 Lấy danh sách chi tiết đơn hàng theo OrderId
        /// </summary>
        public async Task<ApiResponse<PagedResponse<OrderDetail>>> GetByOrderIdAsync(int orderId, int pageNow = 1, int pageSize = 10)
        {
            var search = new SearchOrderDetail { OrderId = orderId };

            var response = await _httpClient.PostAsJsonAsync(
                $"{ApiConstants.OrderDetailApi}/search?pageNow={pageNow}&pageSize={pageSize}",
                search
            );

            return await response.Content.ReadFromJsonAsync<ApiResponse<PagedResponse<OrderDetail>>>(_options)
                   ?? new ApiResponse<PagedResponse<OrderDetail>>
                   {
                       Success = false,
                       Message = "Không parse được dữ liệu từ API"
                   };
        }

        /// <summary>
        /// 🚀 Lấy danh sách chi tiết đơn hàng có phân trang
        /// </summary>
        public async Task<ApiResponse<PagedResponse<OrderDetail>>> GetOrderDetailPageAsync(int pageNow = 1, int pageSize = 10)
        {
            return await _httpClient.GetFromJsonAsync<ApiResponse<PagedResponse<OrderDetail>>>(
                $"{ApiConstants.OrderDetailApi}/paged?pageNow={pageNow}&pageSize={pageSize}", _options
            ) ?? new ApiResponse<PagedResponse<OrderDetail>>
            {
                Success = false,
                Message = "Không parse được dữ liệu từ API"
            };
        }

        /// <summary>
        /// 🚀 Lấy tất cả chi tiết đơn hàng
        /// </summary>
        public async Task<ApiResponse<IEnumerable<OrderDetail>>> GetAllOrderDetailsAsync()
        {
            return await _httpClient.GetFromJsonAsync<ApiResponse<IEnumerable<OrderDetail>>>(
                $"{ApiConstants.OrderDetailApi}", _options
            ) ?? new ApiResponse<IEnumerable<OrderDetail>>
            {
                Success = false,
                Message = "Không parse được dữ liệu từ API"
            };
        }

        /// <summary>
        /// 🚀 Lấy chi tiết đơn hàng theo ID
        /// </summary>
        public async Task<ApiResponse<OrderDetail>> GetOrderDetailByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<ApiResponse<OrderDetail>>(
                $"{ApiConstants.OrderDetailApi}/{id}", _options
            ) ?? new ApiResponse<OrderDetail>
            {
                Success = false,
                Message = "Không parse được dữ liệu từ API"
            };
        }

        /// <summary>
        /// 🚀 Tạo chi tiết đơn hàng
        /// </summary>
        public async Task<ApiResponse<OrderDetail>> CreateOrderDetailAsync(OrderDetail model)
        {
            var response = await _httpClient.PostAsJsonAsync($"{ApiConstants.OrderDetailApi}", model);
            return await response.Content.ReadFromJsonAsync<ApiResponse<OrderDetail>>(_options)
                   ?? new ApiResponse<OrderDetail>
                   {
                       Success = false,
                       Message = "Không parse được dữ liệu từ API"
                   };
        }

        /// <summary>
        /// 🚀 Cập nhật chi tiết đơn hàng
        /// </summary>
        public async Task<ApiResponse<OrderDetail>> UpdateOrderDetailAsync(int id, OrderDetail model)
        {
            var response = await _httpClient.PutAsJsonAsync($"{ApiConstants.OrderDetailApi}/{id}", model);
            return await response.Content.ReadFromJsonAsync<ApiResponse<OrderDetail>>(_options)
                   ?? new ApiResponse<OrderDetail>
                   {
                       Success = false,
                       Message = "Không parse được dữ liệu từ API"
                   };
        }

        /// <summary>
        /// 🚀 Xóa chi tiết đơn hàng
        /// </summary>
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
