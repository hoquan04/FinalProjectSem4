using AdminWeb.Areas.Admin.Data;
using AdminWeb.Areas.Admin.Data.RestAPI;
using AdminWeb.Areas.Admin.Models;
using AdminWeb.Areas.Admin.Models.DTOs;
using System.Text.Json;

public class OrderDetailService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _options;

    public OrderDetailService(IHttpClientFactory factory)
    {
        _httpClient = factory.CreateClient("APIClient");
    }

    // Lấy danh sách chi tiết đơn hàng theo OrderId
    public async Task<APIRespone<AdminWeb.Areas.Admin.Data.RestAPI.PagedResponse<OrderDetail>>> GetByOrderIdAsync(int orderId, int pageNow = 1, int pageSize = 10)
    {
        var search = new SearchOrderDetail { OrderId = orderId };
        var response = await _httpClient.PostAsJsonAsync(
            $"{ApiConstants.OrderApi}detail/search?pageNow={pageNow}&pageSize={pageSize}", search
        );
        return await response.Content.ReadFromJsonAsync<APIRespone<AdminWeb.Areas.Admin.Data.RestAPI.PagedResponse<OrderDetail>>>(_options);
    }

    public async Task<APIRespone<AdminWeb.Areas.Admin.Data.RestAPI.PagedResponse<OrderDetail>>> GetOrderDetailPageAsync(int pageNow = 1, int pageSize = 10)
    {
        return await _httpClient.GetFromJsonAsync<APIRespone<AdminWeb.Areas.Admin.Data.RestAPI.PagedResponse<OrderDetail>>>(
            $"{ApiConstants.OrderApi}detail/paged?pageNow={pageNow}&pageSize={pageSize}");
    }
}
