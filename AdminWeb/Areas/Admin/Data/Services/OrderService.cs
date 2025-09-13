    using AdminWeb.Areas.Admin.Models;
    using AdminWeb.Areas.Admin.Data.RestAPI;


    using System.Text.Json;
    using AdminWeb.Areas.Admin.Data.RestAPI;
    using AdminWeb.Areas.Admin.Models.DTOs;

    namespace AdminWeb.Areas.Admin.Data.Services
    {
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
                return await _httpClient.GetFromJsonAsync<APIRespone<IEnumerable<Order>>>("api/order");
            }

            public async Task<APIRespone<Order>> GetOrderByIdAsync(int id)
            {
                return await _httpClient.GetFromJsonAsync<APIRespone<Order>>($"api/order/{id}");
            }

            public async Task<APIRespone<Order>> CreateOrderAsync(Order model)
            {
                var response = await _httpClient.PostAsJsonAsync("api/order", model);
                return await response.Content.ReadFromJsonAsync<APIRespone<Order>>(_options);
            }

            public async Task<APIRespone<Order>> UpdateOrderAsync(int id, Order model)
            {
                var response = await _httpClient.PutAsJsonAsync($"api/order/{id}", model);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    return new APIRespone<Order>
                    {
                        Success = false,
                        Message = $"API lỗi {response.StatusCode}: {error}"
                    };
                }

                return await response.Content.ReadFromJsonAsync<APIRespone<Order>>(_options);
            }


        public async Task<APIRespone<bool>> DeleteOrderAsync(int id)
            {
                var response = await _httpClient.DeleteAsync($"api/order/{id}");
                return await response.Content.ReadFromJsonAsync<APIRespone<bool>>(_options);
            }

            public async Task<APIRespone<PagedResponse<Order>>> GetOrderPageAsync(int pageNow = 1, int pageSize = 10)
            {
                return await _httpClient.GetFromJsonAsync<APIRespone<PagedResponse<Order>>>(
                    $"api/order/page?pageNow={pageNow}&pageSize={pageSize}");
            }

            public async Task<APIRespone<PagedResponse<Order>>> SearchOrderAsync(SearchOrder search, int pageNow = 1, int pageSize = 10)
            {
                var response = await _httpClient.PostAsJsonAsync(
                    $"api/order/search?pageNow={pageNow}&pageSize={pageSize}", search
                );

                // Nếu API trả về lỗi, đọc raw string để debug
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    return new APIRespone<PagedResponse<Order>>
                    {
                        Success = false,
                        Message = $"API lỗi {response.StatusCode}: {error}",
                        Data = null
                    };
                }

                // Nếu thành công thì parse JSON
                return await response.Content.ReadFromJsonAsync<APIRespone<PagedResponse<Order>>>(_options);
            }


            #endregion

            #region ====== ORDER DETAIL ======

            public async Task<APIRespone<IEnumerable<OrderDetail>>> GetAllOrderDetailsAsync()
            {
                return await _httpClient.GetFromJsonAsync<APIRespone<IEnumerable<OrderDetail>>>("api/orderdetail");
            }

            public async Task<APIRespone<OrderDetail>> GetOrderDetailByIdAsync(int id)
            {
                return await _httpClient.GetFromJsonAsync<APIRespone<OrderDetail>>($"api/orderdetail/{id}");
            }

            public async Task<APIRespone<OrderDetail>> CreateOrderDetailAsync(OrderDetail model)
            {
                var response = await _httpClient.PostAsJsonAsync("api/orderdetail", model);
                return await response.Content.ReadFromJsonAsync<APIRespone<OrderDetail>>(_options);
            }

            public async Task<APIRespone<OrderDetail>> UpdateOrderDetailAsync(int id, OrderDetail model)
            {
                var response = await _httpClient.PutAsJsonAsync($"api/orderdetail/{id}", model);
                return await response.Content.ReadFromJsonAsync<APIRespone<OrderDetail>>(_options);
            }

            public async Task<APIRespone<bool>> DeleteOrderDetailAsync(int id)
            {
                var response = await _httpClient.DeleteAsync($"api/orderdetail/{id}");
                return await response.Content.ReadFromJsonAsync<APIRespone<bool>>(_options);
            }

            public async Task<APIRespone<PagedResponse<OrderDetail>>> GetOrderDetailPageAsync(int pageNow = 1, int pageSize = 10)
            {
                return await _httpClient.GetFromJsonAsync<APIRespone<PagedResponse<OrderDetail>>>(
                    $"api/orderdetail/paged?pageNow={pageNow}&pageSize={pageSize}");
            }

     

            #endregion
        }
    }
