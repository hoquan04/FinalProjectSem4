using AdminWeb.Areas.Admin.Models;
using Newtonsoft.Json;
using System.Text;

namespace AdminWeb.Areas.Admin.Data.Services
{
    public interface IReviewApiService
    {
        Task<ApiResponse<IEnumerable<ReviewViewModel>>> GetAllAsync();
        Task<ApiResponse<ReviewViewModel>> GetByIdAsync(int id);
        Task<ApiResponse<ReviewViewModel>> CreateAsync(CreateReviewViewModel model);
        Task<ApiResponse<ReviewViewModel>> UpdateAsync(int id, UpdateReviewViewModel model);
        Task<ApiResponse<bool>> DeleteAsync(int id);
        Task<ApiResponse<PagedResponse<ReviewViewModel>>> GetPagedAsync(int page, int pageSize);
        Task<ApiResponse<IEnumerable<ReviewViewModel>>> GetByProductIdAsync(int productId);
        Task<ApiResponse<IEnumerable<ReviewViewModel>>> GetByUserIdAsync(int userId);
        Task<ApiResponse<double>> GetAverageRatingByProductIdAsync(int productId);
        Task<ApiResponse<PagedResponse<ReviewViewModel>>> SearchAsync(ReviewSearchViewModel searchModel);
    }

    public class ReviewApiService : IReviewApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public ReviewApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _baseUrl = ApiConstants.ReviewApi;
        }

        public async Task<ApiResponse<IEnumerable<ReviewViewModel>>> GetAllAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync(_baseUrl);
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<IEnumerable<ReviewViewModel>>>(content);
                    return apiResponse ?? new ApiResponse<IEnumerable<ReviewViewModel>>
                    {
                        Success = false,
                        Message = "Không thể đọc dữ liệu từ API"
                    };
                }

                return new ApiResponse<IEnumerable<ReviewViewModel>>
                {
                    Success = false,
                    Message = $"API trả về lỗi: {response.StatusCode}"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<ReviewViewModel>>
                {
                    Success = false,
                    Message = $"Lỗi kết nối API: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<ReviewViewModel>> GetByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/{id}");
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<ReviewViewModel>>(content);
                    return apiResponse ?? new ApiResponse<ReviewViewModel>
                    {
                        Success = false,
                        Message = "Không thể đọc dữ liệu từ API"
                    };
                }

                return new ApiResponse<ReviewViewModel>
                {
                    Success = false,
                    Message = $"API trả về lỗi: {response.StatusCode}"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<ReviewViewModel>
                {
                    Success = false,
                    Message = $"Lỗi kết nối API: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<ReviewViewModel>> CreateAsync(CreateReviewViewModel model)
        {
            try
            {
                var json = JsonConvert.SerializeObject(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(_baseUrl, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<ReviewViewModel>>(responseContent);
                    return apiResponse ?? new ApiResponse<ReviewViewModel>
                    {
                        Success = false,
                        Message = "Không thể đọc dữ liệu từ API"
                    };
                }

                return new ApiResponse<ReviewViewModel>
                {
                    Success = false,
                    Message = $"API trả về lỗi: {response.StatusCode}"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<ReviewViewModel>
                {
                    Success = false,
                    Message = $"Lỗi kết nối API: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<ReviewViewModel>> UpdateAsync(int id, UpdateReviewViewModel model)
        {
            try
            {
                var json = JsonConvert.SerializeObject(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"{_baseUrl}/{id}", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<ReviewViewModel>>(responseContent);
                    return apiResponse ?? new ApiResponse<ReviewViewModel>
                    {
                        Success = false,
                        Message = "Không thể đọc dữ liệu từ API"
                    };
                }

                return new ApiResponse<ReviewViewModel>
                {
                    Success = false,
                    Message = $"API trả về lỗi: {response.StatusCode}"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<ReviewViewModel>
                {
                    Success = false,
                    Message = $"Lỗi kết nối API: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_baseUrl}/{id}");
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<bool>>(content);
                    return apiResponse ?? new ApiResponse<bool>
                    {
                        Success = false,
                        Message = "Không thể đọc dữ liệu từ API"
                    };
                }

                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = $"API trả về lỗi: {response.StatusCode}"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = $"Lỗi kết nối API: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<PagedResponse<ReviewViewModel>>> GetPagedAsync(int page, int pageSize)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/page?pageNow={page}&pageSize={pageSize}");
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<PagedResponse<ReviewViewModel>>>(content);
                    return apiResponse ?? new ApiResponse<PagedResponse<ReviewViewModel>>
                    {
                        Success = false,
                        Message = "Không thể đọc dữ liệu từ API"
                    };
                }

                return new ApiResponse<PagedResponse<ReviewViewModel>>
                {
                    Success = false,
                    Message = $"API trả về lỗi: {response.StatusCode}"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<PagedResponse<ReviewViewModel>>
                {
                    Success = false,
                    Message = $"Lỗi kết nối API: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<IEnumerable<ReviewViewModel>>> GetByProductIdAsync(int productId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/product/{productId}");
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<IEnumerable<ReviewViewModel>>>(content);
                    return apiResponse ?? new ApiResponse<IEnumerable<ReviewViewModel>>
                    {
                        Success = false,
                        Message = "Không thể đọc dữ liệu từ API"
                    };
                }

                return new ApiResponse<IEnumerable<ReviewViewModel>>
                {
                    Success = false,
                    Message = $"API trả về lỗi: {response.StatusCode}"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<ReviewViewModel>>
                {
                    Success = false,
                    Message = $"Lỗi kết nối API: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<IEnumerable<ReviewViewModel>>> GetByUserIdAsync(int userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/user/{userId}");
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<IEnumerable<ReviewViewModel>>>(content);
                    return apiResponse ?? new ApiResponse<IEnumerable<ReviewViewModel>>
                    {
                        Success = false,
                        Message = "Không thể đọc dữ liệu từ API"
                    };
                }

                return new ApiResponse<IEnumerable<ReviewViewModel>>
                {
                    Success = false,
                    Message = $"API trả về lỗi: {response.StatusCode}"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<ReviewViewModel>>
                {
                    Success = false,
                    Message = $"Lỗi kết nối API: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<double>> GetAverageRatingByProductIdAsync(int productId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/average/{productId}");
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<double>>(content);
                    return apiResponse ?? new ApiResponse<double>
                    {
                        Success = false,
                        Message = "Không thể đọc dữ liệu từ API"
                    };
                }

                return new ApiResponse<double>
                {
                    Success = false,
                    Message = $"API trả về lỗi: {response.StatusCode}"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<double>
                {
                    Success = false,
                    Message = $"Lỗi kết nối API: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<PagedResponse<ReviewViewModel>>> SearchAsync(ReviewSearchViewModel searchModel)
        {
            try
            {
                var queryParams = new List<string>();

                if (!string.IsNullOrEmpty(searchModel.SearchKeyword))
                    queryParams.Add($"searchKeyword={Uri.EscapeDataString(searchModel.SearchKeyword)}");

                queryParams.Add($"pageNow={searchModel.PageNumber}");
                queryParams.Add($"pageSize={searchModel.PageSize}");

                var queryString = string.Join("&", queryParams);
                var url = $"{_baseUrl}/search?{queryString}";

                var response = await _httpClient.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<PagedResponse<ReviewViewModel>>>(content);
                    return apiResponse ?? new ApiResponse<PagedResponse<ReviewViewModel>>
                    {
                        Success = false,
                        Message = "Không thể đọc dữ liệu từ API"
                    };
                }

                return new ApiResponse<PagedResponse<ReviewViewModel>>
                {
                    Success = false,
                    Message = $"API trả về lỗi: {response.StatusCode}"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<PagedResponse<ReviewViewModel>>
                {
                    Success = false,
                    Message = $"Lỗi kết nối API: {ex.Message}"
                };
            }
        }
    }
}
