using API.Data;
using API.Models;
using API.Models.DTOs;
using API.Repositories.IRepositories;
using API.Repositories.RestAPI;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories
{
    public class OrderDetailRepository : IOrderDetailRepository
    {
        private readonly DataContext _context;

        public OrderDetailRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<APIRespone<OrderDetail>> AddAsync(OrderDetail entity)
        {
            _context.OrderDetails.Add(entity);
            await _context.SaveChangesAsync();

            return new APIRespone<OrderDetail>
            {
                Success = true,
                Message = "Thêm chi tiết đơn hàng thành công",
                Data = entity
            };
        }

        public async Task<APIRespone<bool>> DeleteAsync(int id)
        {
            var detail = await _context.OrderDetails.FindAsync(id);
            if (detail == null)
            {
                return new APIRespone<bool>
                {
                    Success = false,
                    Message = "Không tìm thấy chi tiết đơn hàng",
                    Data = false
                };
            }

            _context.OrderDetails.Remove(detail);
            await _context.SaveChangesAsync();

            return new APIRespone<bool>
            {
                Success = true,
                Message = "Xóa chi tiết đơn hàng thành công",
                Data = true
            };
        }

        public async Task<APIRespone<IEnumerable<OrderDetail>>> GetAllAsync()
        {
            var details = await _context.OrderDetails
                .Include(od => od.Order)
                .Include(od => od.Product)
                .OrderByDescending(od => od.OrderDetailId)
                .ToListAsync();

            return new APIRespone<IEnumerable<OrderDetail>>
            {
                Success = true,
                Message = "Lấy danh sách chi tiết đơn hàng thành công",
                Data = details
            };
        }

        public async Task<APIRespone<OrderDetail>> GetByIdAsync(int id)
        {
            var detail = await _context.OrderDetails
                .Include(od => od.Order)
                .Include(od => od.Product)
                .FirstOrDefaultAsync(od => od.OrderDetailId == id);

            if (detail == null)
            {
                return new APIRespone<OrderDetail>
                {
                    Success = false,
                    Message = "Không tìm thấy chi tiết đơn hàng",
                    Data = null
                };
            }

            return new APIRespone<OrderDetail>
            {
                Success = true,
                Message = "Lấy chi tiết đơn hàng thành công",
                Data = detail
            };
        }

        public async Task<APIRespone<OrderDetail>> UpdateAsync(int id, OrderDetail entity)
        {
            var detail = await _context.OrderDetails.FindAsync(id);
            if (detail == null)
            {
                return new APIRespone<OrderDetail>
                {
                    Success = false,
                    Message = "Không tìm thấy chi tiết đơn hàng",
                    Data = null
                };
            }

            _context.Entry(detail).CurrentValues.SetValues(entity);
            await _context.SaveChangesAsync();

            return new APIRespone<OrderDetail>
            {
                Success = true,
                Message = "Cập nhật chi tiết đơn hàng thành công",
                Data = entity
            };
        }

        public async Task<APIRespone<PagedResponse<OrderDetail>>> GetPageAsync(int pageNow, int pageSize)
        {
            var query = _context.OrderDetails
                .Include(od => od.Order)
                .Include(od => od.Product)
                .OrderByDescending(od => od.OrderDetailId)
                .AsQueryable();

            var totalCount = await query.CountAsync();
            var totalPage = (int)Math.Ceiling(totalCount / (double)pageSize);

            var data = await query
                .Skip((pageNow - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var response = new PagedResponse<OrderDetail>
            {
                Data = data,
                PageNow = pageNow,
                PageSize = pageSize,
                TotalPage = totalPage,
                TotalCount = totalCount
            };

            return new APIRespone<PagedResponse<OrderDetail>>
            {
                Success = true,
                Message = "Lấy danh sách chi tiết đơn hàng phân trang thành công",
                Data = response
            };
        }

        public async Task<APIRespone<PagedResponse<OrderDetail>>> Search(int pageNow, int pageSize, SearchOrderDetail search)
        {
            var query = _context.OrderDetails
                .Include(od => od.Order)
                .Include(od => od.Product)
                .AsQueryable();

            // Tìm theo từ khóa (ProductName hoặc OrderId)
            if (!string.IsNullOrEmpty(search.Keyword))
            {
                var keyword = search.Keyword.ToLower();
                query = query.Where(od =>
                    od.OrderId.ToString().Contains(keyword) ||
                    od.Product.Name.ToLower().Contains(keyword)
                );
            }

            // Lọc theo OrderId
            if (search.OrderId.HasValue)
            {
                query = query.Where(od => od.OrderId == search.OrderId.Value);
            }

            // Lọc theo ProductId
            if (search.ProductId.HasValue)
            {
                query = query.Where(od => od.ProductId == search.ProductId.Value);
            }

            // Lọc theo số lượng
            if (search.MinQuantity.HasValue)
            {
                query = query.Where(od => od.Quantity >= search.MinQuantity.Value);
            }
            if (search.MaxQuantity.HasValue)
            {
                query = query.Where(od => od.Quantity <= search.MaxQuantity.Value);
            }

            // Lọc theo giá
            if (search.MinPrice.HasValue)
            {
                query = query.Where(od => od.SubTotal >= search.MinPrice.Value);
            }
            if (search.MaxPrice.HasValue)
            {
                query = query.Where(od => od.SubTotal <= search.MaxPrice.Value);
            }

            // Lọc theo ngày
            if (search.FromDate.HasValue)
            {
                query = query.Where(od => od.CreatedDate >= search.FromDate.Value);
            }
            if (search.ToDate.HasValue)
            {
                query = query.Where(od => od.CreatedDate <= search.ToDate.Value);
            }

            var totalCount = await query.CountAsync();
            var totalPage = (int)Math.Ceiling(totalCount / (double)pageSize);

            var data = await query
                .OrderByDescending(od => od.OrderDetailId)
                .Skip((pageNow - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var response = new PagedResponse<OrderDetail>
            {
                Data = data,
                PageNow = pageNow,
                PageSize = pageSize,
                TotalPage = totalPage,
                TotalCount = totalCount
            };

            return new APIRespone<PagedResponse<OrderDetail>>
            {
                Success = true,
                Message = "Tìm kiếm chi tiết đơn hàng thành công",
                Data = response
            };
        }
    }
}
