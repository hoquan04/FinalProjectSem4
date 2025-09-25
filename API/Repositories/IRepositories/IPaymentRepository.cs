using API.Models;
using API.Repositories.RestAPI;

namespace API.Repositories.IRepositories
{
    public interface IPaymentRepository
    {
        // Lấy tất cả payment
        Task<APIRespone<List<Payment>>> GetAllPaymentsAsync();

        // Lấy payment theo id
        Task<APIRespone<Payment>> GetPaymentByIdAsync(int id);

        // Tạo payment mới
        Task<APIRespone<Payment>> CreatePaymentAsync(Payment model);

        // Cập nhật payment theo id
        Task<APIRespone<Payment>> UpdatePaymentAsync(int id, Payment model);

        // Xóa payment theo id
        Task<APIRespone<bool>> DeletePaymentAsync(int id);

        // Tìm kiếm payment (theo từ khóa: có thể là tên người dùng, mã giao dịch, ...)
        Task<APIRespone<List<Payment>>> SearchPaymentsAsync(string searchTerm);

        // Phân trang
        Task<APIRespone<PagedResponse<Payment>>> GetPageAsync(int pageNow, int pageSize);
    }
}
