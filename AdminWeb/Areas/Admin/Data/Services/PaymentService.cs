using System.Net.Http;
using System.Text.Json;
using AdminWeb.Areas.Admin.Models;

namespace AdminWeb.Areas.Admin.Data.Services
{
    public class PaymentService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
        };
        private readonly string _paymentApi = ApiConstants.PaymentApi;

        public PaymentService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Lấy toàn bộ payments
        public async Task<List<Payment>> GetAllPaymentsAsync()
        {
            var resp = await _httpClient.GetAsync(_paymentApi);
            if (!resp.IsSuccessStatusCode) return new List<Payment>();

            var body = await resp.Content.ReadAsStringAsync();
            var apiResp = JsonSerializer.Deserialize<ApiResponse<List<Payment>>>(body, _jsonOptions);
            return apiResp?.Data ?? new List<Payment>();
        }

        // Lấy payment theo id
        public async Task<Payment?> GetPaymentByIdAsync(int id)
        {
            var resp = await _httpClient.GetAsync($"{_paymentApi}/{id}");
            if (!resp.IsSuccessStatusCode) return null;

            var body = await resp.Content.ReadAsStringAsync();
            var apiResp = JsonSerializer.Deserialize<ApiResponse<Payment>>(body, _jsonOptions);
            return apiResp?.Data;
        }

        // Tìm kiếm + lọc theo status
        public async Task<List<Payment>> GetPaymentsAsync(int? searchPaymentId = null, int? searchOrderId = null, PaymentStatus? status = null)
        {
            var payments = await GetAllPaymentsAsync(); // lấy từ API

            if (searchPaymentId.HasValue)
                payments = payments
                    .Where(p => p.PaymentId == searchPaymentId.Value)
                    .ToList();

            if (searchOrderId.HasValue)
                payments = payments
                    .Where(p => p.OrderId == searchOrderId.Value)
                    .ToList();

            if (status.HasValue)
                payments = payments
                    .Where(p => p.PaymentStatus == status.Value)
                    .ToList();

            return payments;
        }
    }
}
