using AdminWeb.Areas.Admin.Models;
using System.Text;
using System.Text.Json;

namespace AdminWeb.Areas.Admin.Data.Services
{
    public class CategoryService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public CategoryService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        /// <summary>
        /// 🚀 GET API - Lấy tất cả categories
        /// </summary>
        public async Task<List<CategoryViewModel>> GetAllCategoriesAsync()
        {
            try
            {
                Console.WriteLine($"📡 Calling GET API: {ApiConstants.CategoryApi}");

                var response = await _httpClient.GetAsync(ApiConstants.CategoryApi);
                var jsonContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"📊 Response Status: {response.StatusCode}");
                Console.WriteLine($"📨 Response Content: {jsonContent}");

                if (response.IsSuccessStatusCode)
                {
                    // Parse response từ API: APIRespone<IEnumerable<Category>>
                    var apiResponse = JsonSerializer.Deserialize<ApiResponse<List<CategoryViewModel>>>(jsonContent, _jsonOptions);
                    return apiResponse?.Data ?? new List<CategoryViewModel>();
                }

                return new List<CategoryViewModel>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error calling GET API: {ex.Message}");
                return new List<CategoryViewModel>();
            }
        }

        /// <summary>
        /// 🚀 GET API - Lấy category theo ID
        /// </summary>
        public async Task<CategoryViewModel?> GetCategoryByIdAsync(int id)
        {
            try
            {
                var url = $"{ApiConstants.CategoryApi}/{id}";
                Console.WriteLine($"📡 Calling GET API: {url}");

                var response = await _httpClient.GetAsync(url);
                var jsonContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"📊 Response Status: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonSerializer.Deserialize<ApiResponse<CategoryViewModel>>(jsonContent, _jsonOptions);
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

        /// <summary>
        /// 🚀 POST API - Tạo category mới
        /// </summary>
        public async Task<ApiResponse<CategoryViewModel>> CreateCategoryAsync(CategoryCreateModel model)
        {
            try
            {
                Console.WriteLine($"create incoming");
                // Chuẩn bị request body JSON
                var requestData = new
                {
                    Name = model.Name,
                    Description = model.Description
                };

                var json = JsonSerializer.Serialize(requestData, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                Console.WriteLine($"📡 Calling POST API: {ApiConstants.CategoryApi}");
                Console.WriteLine($"📦 Request Body: {json}");

                // Gọi API POST
                var response = await _httpClient.PostAsync(ApiConstants.CategoryApi, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"📊 Response Status: {response.StatusCode}");
                Console.WriteLine($"📨 Response Content: {responseContent}");

                // Parse response
                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonSerializer.Deserialize<ApiResponse<CategoryViewModel>>(responseContent, _jsonOptions);
                    return apiResponse ?? new ApiResponse<CategoryViewModel>
                    {
                        Success = false,
                        Message = "Không thể parse response từ API"
                    };
                }
                else
                {
                    // Handle HTTP error
                    var errorResponse = JsonSerializer.Deserialize<ApiResponse<CategoryViewModel>>(responseContent, _jsonOptions);
                    return errorResponse ?? new ApiResponse<CategoryViewModel>
                    {
                        Success = false,
                        Message = $"HTTP Error: {response.StatusCode} - {response.ReasonPhrase}"
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error calling POST API: {ex.Message}");
                return new ApiResponse<CategoryViewModel>
                {
                    Success = false,
                    Message = $"Lỗi kết nối API: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// 🚀 PUT API - Cập nhật category
        /// </summary>
        public async Task<ApiResponse<CategoryViewModel>> UpdateCategoryAsync(int id, CategoryEditModel model)
        {
            try
            {
                // Chuẩn bị request body JSON
                var requestData = new
                {
                    Name = model.Name,
                    Description = model.Description
                };

                var json = JsonSerializer.Serialize(requestData, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var url = $"{ApiConstants.CategoryApi}/{id}";
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
                    var apiResponse = JsonSerializer.Deserialize<ApiResponse<CategoryViewModel>>(responseContent, _jsonOptions);
                    return apiResponse ?? new ApiResponse<CategoryViewModel>
                    {
                        Success = false,
                        Message = "Không thể parse response từ API"
                    };
                }
                else
                {
                    var errorResponse = JsonSerializer.Deserialize<ApiResponse<CategoryViewModel>>(responseContent, _jsonOptions);
                    return errorResponse ?? new ApiResponse<CategoryViewModel>
                    {
                        Success = false,
                        Message = $"HTTP Error: {response.StatusCode} - {response.ReasonPhrase}"
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error calling PUT API: {ex.Message}");
                return new ApiResponse<CategoryViewModel>
                {
                    Success = false,
                    Message = $"Lỗi kết nối API: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// 🚀 DELETE API - Xóa category
        /// </summary>
        public async Task<ApiResponse<bool>> DeleteCategoryAsync(int id)
        {
            try
            {
                var url = $"{ApiConstants.CategoryApi}/{id}";
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

        /// <summary>
        /// 🚀 GET API - Lấy categories với phân trang
        /// </summary>
        public async Task<PagedResponse<CategoryViewModel>> GetPagedCategoriesAsync(int pageNow = 1, int pageSize = 10)
        {
            try
            {
                var url = $"{ApiConstants.CategoryApi}/page?pageNow={pageNow}&pageSize={pageSize}";
                Console.WriteLine($"📡 Calling GET Paged API: {url}");

                var response = await _httpClient.GetAsync(url);
                var jsonContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"📊 Response Status: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    // Parse response từ API: APIRespone<PagedResponse<Category>>
                    var apiResponse = JsonSerializer.Deserialize<ApiResponse<PagedResponse<CategoryViewModel>>>(jsonContent, _jsonOptions);
                    return apiResponse?.Data ?? new PagedResponse<CategoryViewModel>();
                }

                return new PagedResponse<CategoryViewModel>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error calling GET Paged API: {ex.Message}");
                return new PagedResponse<CategoryViewModel>();
            }
        }

        /// <summary>
        /// 🚀 GET API - Lấy categories phân trang (PagedResponse) cho AdminWeb
        /// </summary>
        public async Task<PagedResponse<CategoryViewModel>> GetCategoriesPagedAsync(int pageNow = 1, int pageSize = 10)
        {
            var url = $"{ApiConstants.CategoryApi}/admin/page?pageNow={pageNow}&pageSize={pageSize}";
            try
            {
                Console.WriteLine($"📡 [GetCategoriesPagedAsync] Calling API: {url}");
                var response = await _httpClient.GetAsync(url);
                var jsonContent = await response.Content.ReadAsStringAsync();
                
                Console.WriteLine($"📊 [GetCategoriesPagedAsync] Response Status: {response.StatusCode}");
                Console.WriteLine($"📨 [GetCategoriesPagedAsync] Response Content: {jsonContent}");
                
                if (response.IsSuccessStatusCode && !string.IsNullOrEmpty(jsonContent))
                {
                    try
                    {
                        var apiResponse = JsonSerializer.Deserialize<ApiResponse<PagedResponse<CategoryViewModel>>>(jsonContent, _jsonOptions);
                        Console.WriteLine($"✅ [GetCategoriesPagedAsync] API Success: {apiResponse?.Success}");
                        Console.WriteLine($"📦 [GetCategoriesPagedAsync] Data Count: {apiResponse?.Data?.Data?.Count ?? 0}");
                        Console.WriteLine($"📄 [GetCategoriesPagedAsync] Page Info: {apiResponse?.Data?.PageNow}/{apiResponse?.Data?.TotalPage} (Total: {apiResponse?.Data?.TotalCount})");
                        return apiResponse?.Data ?? new PagedResponse<CategoryViewModel>();
                    }
                    catch (JsonException jsonEx)
                    {
                        Console.WriteLine($"❌ [GetCategoriesPagedAsync] JSON Parse Error: {jsonEx.Message}");
                    }
                }
                else
                {
                    Console.WriteLine($"❌ [GetCategoriesPagedAsync] Error Response: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [GetCategoriesPagedAsync] Exception: {ex.Message}");
            }
            return new PagedResponse<CategoryViewModel>();
        }

        /// <summary>
        /// 🔍 Search categories
        /// </summary>
        public async Task<List<CategoryViewModel>> SearchCategoriesAsync(CategorySearchModel searchModel)
        {
            try
            {
                // Lấy tất cả categories từ API rồi filter local
                var allCategories = await GetAllCategoriesAsync();

                if (string.IsNullOrEmpty(searchModel.SearchTerm))
                    return allCategories;

                return allCategories
                    .Where(c => c.Name.Contains(searchModel.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                               (c.Description?.Contains(searchModel.SearchTerm, StringComparison.OrdinalIgnoreCase) ?? false))
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error searching categories: {ex.Message}");
                return new List<CategoryViewModel>();
            }
        }
    }
}
