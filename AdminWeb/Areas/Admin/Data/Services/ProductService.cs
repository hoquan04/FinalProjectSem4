using AdminWeb.Areas.Admin.Models;
using System.Text;
using System.Text.Json;

namespace AdminWeb.Areas.Admin.Data.Services
{
    public class ProductService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public ProductService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        /// <summary>
        /// 🚀 GET API - Lấy tất cả products
        /// </summary>
        public async Task<List<ProductViewModel>> GetAllProductsAsync()
        {
            try
            {
                Console.WriteLine($"📡 [ProductService.GetAllProductsAsync] Calling GET API: {ApiConstants.ProductApi}");

                var response = await _httpClient.GetAsync(ApiConstants.ProductApi);
                var jsonContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"📊 [ProductService.GetAllProductsAsync] Response Status: {response.StatusCode}");
                Console.WriteLine($"📨 [ProductService.GetAllProductsAsync] Response Length: {jsonContent?.Length ?? 0} chars");
                
                // SỬA LỖI: Kiểm tra null trước khi sử dụng
                if (!string.IsNullOrEmpty(jsonContent))
                {
                    var previewLength = Math.Min(500, jsonContent.Length);
                    Console.WriteLine($"📨 [ProductService.GetAllProductsAsync] Response Content (first {previewLength} chars): {jsonContent.Substring(0, previewLength)}...");
                }

                if (response.IsSuccessStatusCode && !string.IsNullOrEmpty(jsonContent))
                {
                    Console.WriteLine($"✅ [ProductService.GetAllProductsAsync] Success response - deserializing...");
                    
                    try
                    {
                        var apiResponse = JsonSerializer.Deserialize<ApiResponse<List<ProductViewModel>>>(jsonContent, _jsonOptions);
                        
                        Console.WriteLine($"🎯 [ProductService.GetAllProductsAsync] API Response - Success: {apiResponse?.Success}");
                        Console.WriteLine($"🎯 [ProductService.GetAllProductsAsync] API Response - Message: {apiResponse?.Message}");
                        Console.WriteLine($"🎯 [ProductService.GetAllProductsAsync] API Response - Data Count: {apiResponse?.Data?.Count ?? 0}");
                        
                        if (apiResponse?.Data != null && apiResponse.Data.Any())
                        {
                            foreach (var product in apiResponse.Data.Take(3)) // Log first 3 products
                            {
                                Console.WriteLine($"   📦 Product: ID={product.ProductId}, Name={product.Name}, CategoryId={product.CategoryId}");
                            }
                        }
                        
                        return apiResponse?.Data ?? new List<ProductViewModel>();
                    }
                    catch (JsonException jsonEx)
                    {
                        Console.WriteLine($"❌ [ProductService.GetAllProductsAsync] JSON Deserialization Error: {jsonEx.Message}");
                        Console.WriteLine($"❌ [ProductService.GetAllProductsAsync] Raw JSON: {jsonContent}");
                        return new List<ProductViewModel>();
                    }
                }
                else
                {
                    Console.WriteLine($"❌ [ProductService.GetAllProductsAsync] Error response: {response.StatusCode}");
                    Console.WriteLine($"❌ [ProductService.GetAllProductsAsync] Error content: {jsonContent ?? "null"}");
                }

                return new List<ProductViewModel>();
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"🌐 [ProductService.GetAllProductsAsync] HTTP Request Exception: {httpEx.Message}");
                Console.WriteLine($"🌐 [ProductService.GetAllProductsAsync] API URL: {ApiConstants.ProductApi}");
                return new List<ProductViewModel>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [ProductService.GetAllProductsAsync] General Exception: {ex.Message}");
                Console.WriteLine($"❌ [ProductService.GetAllProductsAsync] Stack Trace: {ex.StackTrace}");
                return new List<ProductViewModel>();
            }
        }

        /// <summary>
        /// 🚀 GET API - Lấy product theo ID
        /// </summary>
        public async Task<ProductViewModel?> GetProductByIdAsync(int id)
        {
            try
            {
                var url = $"{ApiConstants.ProductApi}/{id}";
                Console.WriteLine($"📡 Calling GET API: {url}");

                var response = await _httpClient.GetAsync(url);
                var jsonContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"📊 Response Status: {response.StatusCode}");

                if (response.IsSuccessStatusCode && !string.IsNullOrEmpty(jsonContent))
                {
                    var apiResponse = JsonSerializer.Deserialize<ApiResponse<ProductViewModel>>(jsonContent, _jsonOptions);
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
        /// 🚀 POST API - Tạo product mới
        /// </summary>
        public async Task<ApiResponse<ProductViewModel>> CreateProductAsync(ProductCreateModel model)
        {
            try
            {
                Console.WriteLine($"🚀 Creating product: {model.Name}");
                
                var requestData = new
                {
                    Name = model.Name,
                    Description = model.Description,
                    Price = model.Price,
                    StockQuantity = model.StockQuantity,
                    ImageUrl = model.ImageUrl,
                    CategoryId = model.CategoryId
                };

                var json = JsonSerializer.Serialize(requestData, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                Console.WriteLine($"📡 Calling POST API: {ApiConstants.ProductApi}");
                Console.WriteLine($"📦 Request Body: {json}");

                var response = await _httpClient.PostAsync(ApiConstants.ProductApi, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"📊 Response Status: {response.StatusCode}");
                Console.WriteLine($"📨 Response Content: {responseContent}");

                if (response.IsSuccessStatusCode && !string.IsNullOrEmpty(responseContent))
                {
                    var apiResponse = JsonSerializer.Deserialize<ApiResponse<ProductViewModel>>(responseContent, _jsonOptions);
                    return apiResponse ?? new ApiResponse<ProductViewModel>
                    {
                        Success = false,
                        Message = "Không thể parse response từ API"
                    };
                }
                else
                {
                    if (!string.IsNullOrEmpty(responseContent))
                    {
                        var errorResponse = JsonSerializer.Deserialize<ApiResponse<ProductViewModel>>(responseContent, _jsonOptions);
                        return errorResponse ?? new ApiResponse<ProductViewModel>
                        {
                            Success = false,
                            Message = $"HTTP Error: {response.StatusCode} - {response.ReasonPhrase}"
                        };
                    }
                    
                    return new ApiResponse<ProductViewModel>
                    {
                        Success = false,
                        Message = $"HTTP Error: {response.StatusCode} - {response.ReasonPhrase}"
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error calling POST API: {ex.Message}");
                return new ApiResponse<ProductViewModel>
                {
                    Success = false,
                    Message = $"Lỗi kết nối API: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// 🚀 PUT API - Cập nhật product
        /// </summary>
        public async Task<ApiResponse<ProductViewModel>> UpdateProductAsync(int id, ProductEditModel model)
        {
            try
            {
                var requestData = new
                {
                    Name = model.Name,
                    Description = model.Description,
                    Price = model.Price,
                    StockQuantity = model.StockQuantity,
                    ImageUrl = model.ImageUrl,
                    CategoryId = model.CategoryId
                };

                var json = JsonSerializer.Serialize(requestData, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var url = $"{ApiConstants.ProductApi}/{id}";
                Console.WriteLine($"📡 Calling PUT API: {url}");
                Console.WriteLine($"📦 Request Body: {json}");

                var response = await _httpClient.PutAsync(url, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"📊 Response Status: {response.StatusCode}");
                Console.WriteLine($"📨 Response Content: {responseContent}");

                if (response.IsSuccessStatusCode && !string.IsNullOrEmpty(responseContent))
                {
                    var apiResponse = JsonSerializer.Deserialize<ApiResponse<ProductViewModel>>(responseContent, _jsonOptions);
                    return apiResponse ?? new ApiResponse<ProductViewModel>
                    {
                        Success = false,
                        Message = "Không thể parse response từ API"
                    };
                }
                else
                {
                    if (!string.IsNullOrEmpty(responseContent))
                    {
                        var errorResponse = JsonSerializer.Deserialize<ApiResponse<ProductViewModel>>(responseContent, _jsonOptions);
                        return errorResponse ?? new ApiResponse<ProductViewModel>
                        {
                            Success = false,
                            Message = $"HTTP Error: {response.StatusCode} - {response.ReasonPhrase}"
                        };
                    }
                    
                    return new ApiResponse<ProductViewModel>
                    {
                        Success = false,
                        Message = $"HTTP Error: {response.StatusCode} - {response.ReasonPhrase}"
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error calling PUT API: {ex.Message}");
                return new ApiResponse<ProductViewModel>
                {
                    Success = false,
                    Message = $"Lỗi kết nối API: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// 🚀 DELETE API - Xóa product
        /// </summary>
        public async Task<ApiResponse<bool>> DeleteProductAsync(int id)
        {
            try
            {
                var url = $"{ApiConstants.ProductApi}/{id}";
                Console.WriteLine($"📡 Calling DELETE API: {url}");

                var response = await _httpClient.DeleteAsync(url);
                var responseContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"📊 Response Status: {response.StatusCode}");
                Console.WriteLine($"📨 Response Content: {responseContent}");

                if (response.IsSuccessStatusCode && !string.IsNullOrEmpty(responseContent))
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
                    if (!string.IsNullOrEmpty(responseContent))
                    {
                        var errorResponse = JsonSerializer.Deserialize<ApiResponse<bool>>(responseContent, _jsonOptions);
                        return errorResponse ?? new ApiResponse<bool>
                        {
                            Success = false,
                            Message = $"HTTP Error: {response.StatusCode} - {response.ReasonPhrase}"
                        };
                    }
                    
                    return new ApiResponse<bool>
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
        /// 🔍 Search products
        /// </summary>
        public async Task<List<ProductViewModel>> SearchProductsAsync(ProductSearchModel searchModel)
        {
            try
            {
                var allProducts = await GetAllProductsAsync();

                if (string.IsNullOrEmpty(searchModel.SearchTerm) && !searchModel.CategoryId.HasValue)
                    return allProducts;

                var filteredProducts = allProducts.Where(p => 
                {
                    bool matchesSearch = true;
                    bool matchesCategory = true;

                    if (!string.IsNullOrEmpty(searchModel.SearchTerm))
                    {
                        matchesSearch = p.Name.Contains(searchModel.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                                       (!string.IsNullOrEmpty(p.Description) && p.Description.Contains(searchModel.SearchTerm, StringComparison.OrdinalIgnoreCase)) ||
                                       (p.Category != null && !string.IsNullOrEmpty(p.Category.Name) && p.Category.Name.Contains(searchModel.SearchTerm, StringComparison.OrdinalIgnoreCase));
                    }

                    if (searchModel.CategoryId.HasValue)
                    {
                        matchesCategory = p.CategoryId == searchModel.CategoryId.Value;
                    }

                    return matchesSearch && matchesCategory;
                }).ToList();

                return filteredProducts;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error searching products: {ex.Message}");
                return new List<ProductViewModel>();
            }
        }

        /// <summary>
        /// 🚀 GET API - Lấy products theo category
        /// </summary>
        public async Task<List<ProductViewModel>> GetProductsByCategoryAsync(int categoryId)
        {
            try
            {
                var url = $"{ApiConstants.ProductApi}/category/{categoryId}";
                Console.WriteLine($"📡 Calling GET Products by Category API: {url}");

                var response = await _httpClient.GetAsync(url);
                var jsonContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"📊 Response Status: {response.StatusCode}");

                if (response.IsSuccessStatusCode && !string.IsNullOrEmpty(jsonContent))
                {
                    var apiResponse = JsonSerializer.Deserialize<ApiResponse<List<ProductViewModel>>>(jsonContent, _jsonOptions);
                    return apiResponse?.Data ?? new List<ProductViewModel>();
                }

                return new List<ProductViewModel>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error calling GET Products by Category API: {ex.Message}");
                return new List<ProductViewModel>();
            }
        }
    }
}
