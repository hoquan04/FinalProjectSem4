using AdminWeb.Areas.Admin.Models;
using System.Text;
using System.Text.Json;

namespace AdminWeb.Areas.Admin.Data.Services
{
    public class ShippingApiService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public ShippingApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<List<Shipping>> GetAllShippingAsync()
        {
            try
            {
                

                var response = await _httpClient.GetAsync(ApiConstants.ShippingApi);
                var jsonContent = await response.Content.ReadAsStringAsync();

                

                if (response.IsSuccessStatusCode)
                {
                    // Parse response từ API: APIRespone<IEnumerable<Category>>
                    var apiResponse = JsonSerializer.Deserialize<ApiResponse<List<Shipping>>>(jsonContent, _jsonOptions);
                    return apiResponse?.Data ?? new List<Shipping>();
                }

                return new List<Shipping>();
            }
            catch (Exception ex)
            {
                
                return new List<Shipping>();
            }
        }

        public async Task<Shipping?> GetShippingByIdAsync(int id)
        {
            try
            {
                var url = $"{ApiConstants.ShippingApi}/{id}";
                Console.WriteLine($"📡 Calling GET API: {url}");

                var response = await _httpClient.GetAsync(url);
                var jsonContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"📊 Response Status: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonSerializer.Deserialize<ApiResponse<Shipping>>(jsonContent, _jsonOptions);
                    return apiResponse?.Data;
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error calling GET by ID API: {ex.Message}");
                return null;
            }
        }

        public async Task<ApiResponse<Shipping>> CreateShippingAsync(ShippingCreateModel model)
        {
            try
            {
                Console.WriteLine($"create incoming");
                // Chuẩn bị request body JSON
                var requestData = new
                {
                    RecipientName = model.RecipientName,
                    PhoneNumber = model.PhoneNumber,
                    Email = model.Email,
                    Address = model.Address,
                    City = model.City,
                    PostalCode = model.PostalCode,
                    ShippingFee = model.ShippingFee,
                    EstimatedDays = model.EstimatedDays
                };


                var json = JsonSerializer.Serialize(requestData, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                Console.WriteLine($"📡 Calling POST API: {ApiConstants.ShippingApi}");
                Console.WriteLine($"📦 Request Body: {json}");

                // Gọi API POST
                var response = await _httpClient.PostAsync(ApiConstants.ShippingApi, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"📊 Response Status: {response.StatusCode}");
                Console.WriteLine($"📨 Response Content: {responseContent}");

                // Parse response
                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonSerializer.Deserialize<ApiResponse<Shipping>>(responseContent, _jsonOptions);
                    return apiResponse ?? new ApiResponse<Shipping>
                    {
                        Success = false,
                        Message = "Không thể parse response từ API"
                    };
                }
                else
                {
                    // Handle HTTP error
                    var errorResponse = JsonSerializer.Deserialize<ApiResponse<Shipping>>(responseContent, _jsonOptions);
                    return errorResponse ?? new ApiResponse<Shipping>
                    {
                        Success = false,
                        Message = $"HTTP Error: {response.StatusCode} - {response.ReasonPhrase}"
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error calling POST API: {ex.Message}");
                return new ApiResponse<Shipping>
                {
                    Success = false,
                    Message = $"Lỗi kết nối API: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<Shipping>> UpdateShippingAsync(int id, ShippingEditModel model)
        {
            try
            {
                // Chuẩn bị request body JSON
                var requestData = new
                {
                    RecipientName = model.RecipientName,
                    PhoneNumber = model.PhoneNumber,
                    Email = model.Email,
                    Address = model.Address,
                    City = model.City,
                    PostalCode = model.PostalCode,
                    ShippingFee = model.ShippingFee,
                    EstimatedDays = model.EstimatedDays
                };


                var json = JsonSerializer.Serialize(requestData, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var url = $"{ApiConstants.ShippingApi}/{id}";
                Console.WriteLine($"📡 Calling PUT API: {url}");
                Console.WriteLine($"📦 Request Body: {json}");

                // Gọi API PUT
                var response = await _httpClient.PutAsync(url, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"📊 Response Status: {response.StatusCode}");
                Console.WriteLine($"📨 Response Content: {responseContent}");

                // Parse response
                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonSerializer.Deserialize<ApiResponse<Shipping>>(responseContent, _jsonOptions);
                    return apiResponse ?? new ApiResponse<Shipping>
                    {
                        Success = false,
                        Message = "Không thể parse response từ API"
                    };
                }
                else
                {
                    var errorResponse = JsonSerializer.Deserialize<ApiResponse<Shipping>>(responseContent, _jsonOptions);
                    return errorResponse ?? new ApiResponse<Shipping>
                    {
                        Success = false,
                        Message = $"HTTP Error: {response.StatusCode} - {response.ReasonPhrase}"
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error calling PUT API: {ex.Message}");
                return new ApiResponse<Shipping>
                {
                    Success = false,
                    Message = $"Lỗi kết nối API: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<bool>> DeleteShippingAsync(int id)
        {
            try
            {
                var url = $"{ApiConstants.ShippingApi}/{id}";
                Console.WriteLine($"📡 Calling DELETE API: {url}");

                // Gọi API DELETE
                var response = await _httpClient.DeleteAsync(url);
                var responseContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"📊 Response Status: {response.StatusCode}");
                Console.WriteLine($"📨 Response Content: {responseContent}");

                // Parse response
                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonSerializer.Deserialize<ApiResponse<bool>>(responseContent, _jsonOptions);
                    return apiResponse ?? new ApiResponse<bool>
                    {
                        Success = false,
                        Message = "Không thể parse response từ API"
                    };
                }
                else
                {
                    var errorResponse = JsonSerializer.Deserialize<ApiResponse<bool>>(responseContent, _jsonOptions);
                    return errorResponse ?? new ApiResponse<bool>
                    {
                        Success = false,
                        Message = $"HTTP Error: {response.StatusCode} - {response.ReasonPhrase}"
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error calling DELETE API: {ex.Message}");
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = $"Lỗi kết nối API: {ex.Message}"
                };
            }
        }

        public async Task<PagedResponse<Shipping>> GetPagedShippingAsync(int pageNow = 1, int pageSize = 10)
        {
            try
            {
                var url = $"{ApiConstants.ShippingApi}/page?pageNow={pageNow}&pageSize={pageSize}";
                Console.WriteLine($"📡 Calling GET Paged API: {url}");

                var response = await _httpClient.GetAsync(url);
                var jsonContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"📊 Response Status: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    // Parse response từ API: APIRespone<PagedResponse<Shipping>>
                    var apiResponse = JsonSerializer.Deserialize<ApiResponse<PagedResponse<Shipping>>>(jsonContent, _jsonOptions);
                    return apiResponse?.Data ?? new PagedResponse<Shipping>();
                }

                return new PagedResponse<Shipping>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error calling GET Paged API: {ex.Message}");
                return new PagedResponse<Shipping>();
            }
        }

        public async Task<List<Shipping>> SearchShippingAsync(ShippingSearchModel searchModel)
        {
            try
            {
                var queryParams = new List<string>();

                if (!string.IsNullOrEmpty(searchModel.RecipientName))
                    queryParams.Add($"RecipientName={searchModel.RecipientName}");
                if (!string.IsNullOrEmpty(searchModel.PhoneNumber))
                    queryParams.Add($"PhoneNumber={searchModel.PhoneNumber}");
                if (!string.IsNullOrEmpty(searchModel.Address))
                    queryParams.Add($"Address={searchModel.Address}");
                if (!string.IsNullOrEmpty(searchModel.City))
                    queryParams.Add($"City={searchModel.City}");
                if (!string.IsNullOrEmpty(searchModel.PostalCode))
                    queryParams.Add($"PostalCode={searchModel.PostalCode}");

                // Phân trang
                queryParams.Add($"PageNow={searchModel.PageNow}");
                queryParams.Add($"PageSize={searchModel.PageSize}");

                var url = $"{ApiConstants.ShippingApi}/search";
                if (queryParams.Any())
                    url += "?" + string.Join("&", queryParams);

                Console.WriteLine($"📡 Calling Search API: {url}");

                var response = await _httpClient.GetAsync(url);
                var jsonContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonSerializer.Deserialize<ApiResponse<List<Shipping>>>(jsonContent, _jsonOptions);
                    return apiResponse?.Data ?? new List<Shipping>();
                }

                return new List<Shipping>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error searching shippings: {ex.Message}");
                return new List<Shipping>();
            }
        }


    }
}
