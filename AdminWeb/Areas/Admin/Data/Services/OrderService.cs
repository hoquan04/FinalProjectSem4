using AdminWeb.Areas.Admin.Data;
using AdminWeb.Areas.Admin.Data.RestAPI;
using AdminWeb.Areas.Admin.Models;
using AdminWeb.Areas.Admin.Models.DTOs;
using System.Text.Json;

public class OrderService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _options;

    public OrderService(IHttpClientFactory factory)
    {
        _httpClient = factory.CreateClient("APIClient");
    }

    #region ====== ORDER ======

    public async Task<APIRespone<IEnumerable<Order>>> GetAllOrdersAsync()
    {
        return await _httpClient.GetFromJsonAsync<APIRespone<IEnumerable<Order>>>(ApiConstants.OrderApi);
    }

    public async Task<APIRespone<Order>> GetOrderByIdAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<APIRespone<Order>>($"{ApiConstants.OrderApi}/{id}");
    }

    public async Task<APIRespone<Order>> CreateOrderAsync(Order model)
    {
        var response = await _httpClient.PostAsJsonAsync(ApiConstants.OrderApi, model);
        return await response.Content.ReadFromJsonAsync<APIRespone<Order>>(_options);
    }

    public async Task<APIRespone<Order>> UpdateOrderAsync(int id, Order model)
    {
        var response = await _httpClient.PutAsJsonAsync($"{ApiConstants.OrderApi}/{id}", model);
        return await response.Content.ReadFromJsonAsync<APIRespone<Order>>(_options);
    }

    public async Task<APIRespone<bool>> DeleteOrderAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"{ApiConstants.OrderApi}/{id}");
        return await response.Content.ReadFromJsonAsync<APIRespone<bool>>(_options);
    }

    public async Task<APIRespone<AdminWeb.Areas.Admin.Data.RestAPI.PagedResponse<Order>>> GetOrderPageAsync(int pageNow = 1, int pageSize = 10)
    {
        return await _httpClient.GetFromJsonAsync<APIRespone<AdminWeb.Areas.Admin.Data.RestAPI.PagedResponse<Order>>>(
            $"{ApiConstants.OrderApi}/page?pageNow={pageNow}&pageSize={pageSize}");
    }

    public async Task<APIRespone<AdminWeb.Areas.Admin.Data.RestAPI.PagedResponse<Order>>> SearchOrderAsync(SearchOrder search, int pageNow = 1, int pageSize = 10)
    {
        var response = await _httpClient.PostAsJsonAsync(
            $"{ApiConstants.OrderApi}/search?pageNow={pageNow}&pageSize={pageSize}", search
        );
        return await response.Content.ReadFromJsonAsync<APIRespone<AdminWeb.Areas.Admin.Data.RestAPI.PagedResponse<Order>>>(_options);
    }

    #endregion
}
