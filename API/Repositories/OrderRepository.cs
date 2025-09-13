using API.Data;
using API.Models;
using API.Models.DTOs;
using API.Repositories.IRepositories;
using API.Repositories.RestAPI;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly DataContext _context;

        public OrderRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<APIRespone<Order>> AddAsync(Order entity)
        {
            _context.Orders.Add(entity);
            await _context.SaveChangesAsync();
            return new APIRespone<Order>
            {
                Success = true,
                Message = "Thêm đơn hàng thành công",
                Data = entity
            };
        }

        public async Task<APIRespone<bool>> DeleteAsync(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return new APIRespone<bool>
                {
                    Success = false,
                    Message = "Không tìm thấy đơn hàng",
                    Data = false
                };
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return new APIRespone<bool>
            {
                Success = true,
                Message = "Xóa đơn hàng thành công",
                Data = true
            };
        }

        public async Task<APIRespone<IEnumerable<Order>>> GetAllAsync()
        {
            var orders = await _context.Orders
                .Include(o => o.Users)
                .Include(o => o.Shipping)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .Include(o => o.Payments)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return new APIRespone<IEnumerable<Order>>
            {
                Success = true,
                Message = "Lấy danh sách tất cả đơn hàng thành công",
                Data = orders
            };
        }

        public async Task<APIRespone<Order>> GetByIdAsync(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Users)
                .Include(o => o.Shipping)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .Include(o => o.Payments)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
            {
                return new APIRespone<Order>
                {
                    Success = false,
                    Message = "Không tìm thấy đơn hàng",
                    Data = null
                };
            }

            return new APIRespone<Order>
            {
                Success = true,
                Message = "Lấy đơn hàng thành công",
                Data = order
            };
        }

        public async Task<APIRespone<Order>> UpdateAsync(int id, Order entity)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return new APIRespone<Order>
                {
                    Success = false,
                    Message = "Không tìm thấy đơn hàng",
                    Data = null
                };
            }

            _context.Entry(order).CurrentValues.SetValues(entity);
            await _context.SaveChangesAsync();

            return new APIRespone<Order>
            {
                Success = true,
                Message = "Cập nhật đơn hàng thành công",
                Data = entity
            };
        }

        public async Task<APIRespone<PagedResponse<Order>>> GetPageAsync(int pageNow, int pageSize)
        {
            var query = _context.Orders
                .Include(o => o.Users)
                .Include(o => o.Shipping)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .Include(o => o.Payments)
                .OrderByDescending(o => o.OrderDate)
                .AsQueryable();

            var totalCount = await query.CountAsync();
            var totalPage = (int)Math.Ceiling(totalCount / (double)pageSize);

            var data = await query
                .Skip((pageNow - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var response = new PagedResponse<Order>
            {
                Data = data,
                PageNow = pageNow,
                PageSize = pageSize,
                TotalPage = totalPage,
                TotalCount = totalCount
            };

            return new APIRespone<PagedResponse<Order>>
            {
                Success = true,
                Message = "Lấy danh sách đơn hàng phân trang thành công",
                Data = response
            };
        }

        public async Task<APIRespone<PagedResponse<Order>>> Search(int pageNow, int pageSize, SearchOrder search)
        {
            var query = _context.Orders
                .Include(o => o.Users)
                .Include(o => o.Shipping)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .Include(o => o.Payments)
                .AsQueryable();

            // 🔎 Tìm theo từ khóa
            if (!string.IsNullOrEmpty(search.Keyword))
            {
                var keyword = search.Keyword.ToLower();

                if (int.TryParse(search.Keyword, out int orderId))
                {
                    // Nếu nhập số thì tìm theo Mã đơn
                    query = query.Where(o => o.OrderId == orderId);
                }
                else if (Enum.TryParse<OrderStatus>(search.Keyword, true, out var status))
                {
                    // Nếu nhập đúng tên trạng thái thì tìm theo Trạng thái
                    query = query.Where(o => o.Status == status);
                }
                else
                {
                    // Nếu nhập chữ thì tìm theo tên hoặc email
                    query = query.Where(o =>
                        o.Users.FullName.ToLower().Contains(keyword) ||
                        o.Users.Email.ToLower().Contains(keyword)
                    );
                }
            }

            // 📅 Lọc theo ngày
            // 📅 Lọc theo ngày - SỬA LẠI
            if (search.FromDate.HasValue)
            {
                var from = search.FromDate.Value.Date;
                query = query.Where(o => o.OrderDate >= from);
            }

            if (search.ToDate.HasValue)
            {
                var to = search.ToDate.Value.Date.AddDays(1).AddSeconds(-1);
                query = query.Where(o => o.OrderDate <= to);
            }

            // 💰 Lọc theo tổng tiền
            if (search.MinAmount.HasValue)
            {
                query = query.Where(o => o.TotalAmount >= search.MinAmount.Value);
            }
            if (search.MaxAmount.HasValue)
            {
                query = query.Where(o => o.TotalAmount <= search.MaxAmount.Value);
            }

            // 📌 Phân trang
            var totalCount = await query.CountAsync();
            var totalPage = (int)Math.Ceiling(totalCount / (double)pageSize);

            var data = await query
                .OrderByDescending(o => o.OrderDate)
                .Skip((pageNow - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var response = new PagedResponse<Order>
            {
                Data = data,
                PageNow = pageNow,
                PageSize = pageSize,
                TotalPage = totalPage,
                TotalCount = totalCount
            };

            return new APIRespone<PagedResponse<Order>>
            {
                Success = true,
                Message = "Tìm kiếm đơn hàng thành công",
                Data = response
            };
        }

    }
}
