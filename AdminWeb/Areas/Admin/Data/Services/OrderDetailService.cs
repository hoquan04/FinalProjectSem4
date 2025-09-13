using AdminWeb.Areas.Admin.Data.RestAPI;
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
        }

        // Lấy danh sách chi tiết đơn hàng theo OrderId
        public async Task<APIRespone<PagedResponse<OrderDetail>>> GetByOrderIdAsync(int orderId, int pageNow = 1, int pageSize = 10)
        {
            var search = new SearchOrderDetail { OrderId = orderId };
            var response = await _httpClient.PostAsJsonAsync(
                $"api/orderdetail/search?pageNow={pageNow}&pageSize={pageSize}",
                search
            );
            return await response.Content.ReadFromJsonAsync<APIRespone<PagedResponse<OrderDetail>>>(_options);
        }
    }
}
