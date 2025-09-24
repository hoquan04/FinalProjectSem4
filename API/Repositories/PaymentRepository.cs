using API.Data;
using API.Models;
using API.Repositories.IRepositories;
using API.Repositories.RestAPI;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly DataContext _context;

        public PaymentRepository(DataContext context)
        {
            _context = context;
        }

        // Lấy tất cả payment
        public async Task<APIRespone<List<Payment>>> GetAllPaymentsAsync()
        {
            var response = new APIRespone<List<Payment>>();
            try
            {
                response.Data = await _context.Payments
                    .Include(p => p.Order)
                    .ToListAsync();

                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        // Lấy payment theo id
        public async Task<APIRespone<Payment>> GetPaymentByIdAsync(int id)
        {
            var response = new APIRespone<Payment>();
            try
            {
                var payment = await _context.Payments
                    .Include(p => p.Order)
                    .FirstOrDefaultAsync(p => p.PaymentId == id);

                if (payment == null)
                {
                    response.Success = false;
                    response.Message = "Không tìm thấy Payment";
                }
                else
                {
                    response.Data = payment;
                    response.Success = true;
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        // Tạo payment mới
        public async Task<APIRespone<Payment>> CreatePaymentAsync(Payment model)
        {
            var response = new APIRespone<Payment>();
            try
            {
                model.CreatedAt = DateTime.Now; // set tự động
                _context.Payments.Add(model);
                await _context.SaveChangesAsync();

                response.Data = model;
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        // Cập nhật payment
        public async Task<APIRespone<Payment>> UpdatePaymentAsync(int id, Payment model)
        {
            var response = new APIRespone<Payment>();
            try
            {
                var existing = await _context.Payments.FindAsync(id);
                if (existing == null)
                {
                    response.Success = false;
                    response.Message = "Payment không tồn tại";
                    return response;
                }

                // Chỉ cập nhật các field hợp lệ
                existing.PaymentMethod = model.PaymentMethod;
                existing.PaymentStatus = model.PaymentStatus;

                // Nếu trạng thái chuyển sang Paid => set PaidAt
                if (model.PaymentStatus == PaymentStatus.Paid && existing.PaidAt == null)
                {
                    existing.PaidAt = DateTime.Now;
                }

                _context.Payments.Update(existing);
                await _context.SaveChangesAsync();

                response.Data = existing;
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        // Xóa payment
        public async Task<APIRespone<bool>> DeletePaymentAsync(int id)
        {
            var response = new APIRespone<bool>();
            try
            {
                var payment = await _context.Payments.FindAsync(id);
                if (payment == null)
                {
                    response.Success = false;
                    response.Message = "Payment không tồn tại";
                    return response;
                }

                _context.Payments.Remove(payment);
                await _context.SaveChangesAsync();

                response.Data = true;
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        // Tìm kiếm Payment (ví dụ: theo OrderId hoặc Status)
        public async Task<APIRespone<List<Payment>>> SearchPaymentsAsync(string searchTerm)
        {
            var response = new APIRespone<List<Payment>>();
            try
            {
                int.TryParse(searchTerm, out int orderId);

                response.Data = await _context.Payments
                    .Include(p => p.Order)
                    .Where(p => p.OrderId == orderId
                             || p.PaymentStatus.ToString().Contains(searchTerm)
                             || p.PaymentMethod.ToString().Contains(searchTerm))
                    .ToListAsync();

                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        // Phân trang
        public async Task<APIRespone<PagedResponse<Payment>>> GetPageAsync(int pageNow, int pageSize)
        {
            var response = new APIRespone<PagedResponse<Payment>>();
            try
            {
                var totalRecords = await _context.Payments.CountAsync();

                var items = await _context.Payments
                    .Include(p => p.Order)
                    .OrderByDescending(p => p.CreatedAt)
                    .Skip((pageNow - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                response.Data = new PagedResponse<Payment>
                {
                    Data = items,
                    PageNow = pageNow,
                    PageSize = pageSize,
                    TotalCount = totalRecords,
                    TotalPage = (int)Math.Ceiling((double)totalRecords / pageSize)
                };
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }
    }
}
