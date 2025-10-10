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

            // 🚀 Thêm chi tiết đơn hàng
            public async Task<APIRespone<OrderDetailDto>> AddAsync(OrderDetail entity)
            {
                _context.OrderDetails.Add(entity);
                await _context.SaveChangesAsync();

                var dto = await MapToDto(entity.OrderDetailId);

                return new APIRespone<OrderDetailDto>
                {
                    Success = true,
                    Message = "Thêm chi tiết đơn hàng thành công",
                    Data = dto
                };
            }

            // 🚀 Xóa chi tiết
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

            // 🚀 Lấy tất cả chi tiết (không phân trang)
            public async Task<APIRespone<IEnumerable<OrderDetailDto>>> GetAllAsync()
            {
                var data = await _context.OrderDetails
                    .Include(od => od.Product)
                    .Include(od => od.Order).ThenInclude(o => o.Shipping)
                    .Include(od => od.Order).ThenInclude(o => o.Payments)
                    .Select(od => MapOrderDetailToDto(od))
                    .OrderByDescending(x => x.OrderDetailId)
                    .ToListAsync();

                return new APIRespone<IEnumerable<OrderDetailDto>>
                {
                    Success = true,
                    Message = "Lấy danh sách chi tiết đơn hàng thành công",
                    Data = data
                };
            }

            // 🚀 Lấy chi tiết theo Id
            public async Task<APIRespone<OrderDetailDto>> GetByIdAsync(int id)
            {
                var dto = await MapToDto(id);

                if (dto == null)
                {
                    return new APIRespone<OrderDetailDto>
                    {
                        Success = false,
                        Message = "Không tìm thấy chi tiết đơn hàng",
                        Data = null
                    };
                }

                return new APIRespone<OrderDetailDto>
                {
                    Success = true,
                    Message = "Lấy chi tiết đơn hàng thành công",
                    Data = dto
                };
            }

            // 🚀 Cập nhật chi tiết
            public async Task<APIRespone<OrderDetailDto>> UpdateAsync(int id, OrderDetail entity)
            {
                var detail = await _context.OrderDetails.FindAsync(id);
                if (detail == null)
                {
                    return new APIRespone<OrderDetailDto>
                    {
                        Success = false,
                        Message = "Không tìm thấy chi tiết đơn hàng",
                        Data = null
                    };
                }

                _context.Entry(detail).CurrentValues.SetValues(entity);
                await _context.SaveChangesAsync();

                var dto = await MapToDto(id);

                return new APIRespone<OrderDetailDto>
                {
                    Success = true,
                    Message = "Cập nhật chi tiết đơn hàng thành công",
                    Data = dto
                };
            }

            // 🚀 Lấy danh sách phân trang
            public async Task<APIRespone<PagedResponse<OrderDetailDto>>> GetPageAsync(int pageNow, int pageSize)
            {
                var query = _context.OrderDetails
                    .Include(od => od.Product)
                    .Include(od => od.Order).ThenInclude(o => o.Shipping)
                    .OrderByDescending(od => od.OrderDetailId)
                    .Include(od => od.Order).ThenInclude(o => o.Payments)
                    .AsQueryable();

                var totalCount = await query.CountAsync();
                var totalPage = (int)Math.Ceiling(totalCount / (double)pageSize);

                var data = await query
                    .Skip((pageNow - 1) * pageSize)
                    .Take(pageSize)
                    .Select(od => MapOrderDetailToDto(od))
                    .ToListAsync();

                var response = new PagedResponse<OrderDetailDto>
                {
                    Data = data,
                    PageNow = pageNow,
                    PageSize = pageSize,
                    TotalPage = totalPage,
                    TotalCount = totalCount
                };

                return new APIRespone<PagedResponse<OrderDetailDto>>
                {
                    Success = true,
                    Message = "Lấy danh sách chi tiết đơn hàng phân trang thành công",
                    Data = response
                };
            }

            // 🚀 Search
            public async Task<APIRespone<PagedResponse<OrderDetailDto>>> Search(int pageNow, int pageSize, SearchOrderDetail search)
            {
                var query = _context.OrderDetails
                    .Include(od => od.Product)
                    .Include(od => od.Order).ThenInclude(o => o.Shipping)
                    .Include(od => od.Order).ThenInclude(o => o.Payments)
                    .AsQueryable();

                // 🔎 Tìm theo từ khóa
                if (!string.IsNullOrEmpty(search.Keyword))
                {
                    var keyword = search.Keyword.ToLower();
                    query = query.Where(od =>
                        od.OrderId.ToString().Contains(keyword) ||
                        od.Product.Name.ToLower().Contains(keyword) ||
                        od.Order.Shipping.RecipientName.ToLower().Contains(keyword) ||
                        od.Order.Shipping.Email.ToLower().Contains(keyword) ||
                        od.Order.Shipping.Address.ToLower().Contains(keyword)
                    );
                }

                // Lọc OrderId
                if (search.OrderId.HasValue)
                    query = query.Where(od => od.OrderId == search.OrderId.Value);

                // Lọc ProductId
                if (search.ProductId.HasValue)
                    query = query.Where(od => od.ProductId == search.ProductId.Value);

                // Lọc số lượng
                if (search.MinQuantity.HasValue)
                    query = query.Where(od => od.Quantity >= search.MinQuantity.Value);
                if (search.MaxQuantity.HasValue)
                    query = query.Where(od => od.Quantity <= search.MaxQuantity.Value);

                // Lọc giá
                if (search.MinPrice.HasValue)
                    query = query.Where(od => od.SubTotal >= search.MinPrice.Value);
                if (search.MaxPrice.HasValue)
                    query = query.Where(od => od.SubTotal <= search.MaxPrice.Value);

                // Lọc ngày
                if (search.FromDate.HasValue)
                    query = query.Where(od => od.CreatedDate >= search.FromDate.Value);
                if (search.ToDate.HasValue)
                    query = query.Where(od => od.CreatedDate <= search.ToDate.Value);

                var totalCount = await query.CountAsync();
                var totalPage = (int)Math.Ceiling(totalCount / (double)pageSize);

                var data = await query
                    .OrderByDescending(od => od.OrderDetailId)
                    .Skip((pageNow - 1) * pageSize)
                    .Take(pageSize)
                    .Select(od => MapOrderDetailToDto(od))
                    .ToListAsync();

                var response = new PagedResponse<OrderDetailDto>
                {
                    Data = data,
                    PageNow = pageNow,
                    PageSize = pageSize,
                    TotalPage = totalPage,
                    TotalCount = totalCount
                };

                return new APIRespone<PagedResponse<OrderDetailDto>>
                {
                    Success = true,
                    Message = "Tìm kiếm chi tiết đơn hàng thành công",
                    Data = response
                };
            }

            // Helper map entity -> DTO
            private async Task<OrderDetailDto?> MapToDto(int id)
            {
                return await _context.OrderDetails
                    .Include(od => od.Product)
                    .Include(od => od.Order).ThenInclude(o => o.Shipping)
                    .Include(od => od.Order).ThenInclude(o => o.Payments)
                    .Where(od => od.OrderDetailId == id)
                    .Select(od => MapOrderDetailToDto(od))
                    .FirstOrDefaultAsync();
            }

            // Map function
            private static OrderDetailDto MapOrderDetailToDto(OrderDetail od)
            {
                return new OrderDetailDto
                {
                    OrderDetailId = od.OrderDetailId,
                    ProductId = od.ProductId,
                    ProductName = od.Product.Name,
                    ImageUrl = od.Product.ImageUrl,
                    Quantity = od.Quantity,
                    UnitPrice = od.UnitPrice,
                    SubTotal = od.SubTotal,
                    CreatedDate = od.CreatedDate,

                    // Order info
                    OrderId = od.Order.OrderId,
                    OrderDate = od.Order.OrderDate,
                    Status = od.Order.Status,

                    // Shipping info
                    CustomerName = od.Order.Shipping.RecipientName,
                    Email = od.Order.Shipping.Email,
                    PhoneNumber = od.Order.Shipping.PhoneNumber,
                    Address = od.Order.Shipping.Address,
                    City = od.Order.Shipping.City,
                    PostalCode = od.Order.Shipping.PostalCode,
                    ShippingFee = od.Order.Shipping.ShippingFee,
                    PaymentStatus = od.Order.Payments
                    .OrderByDescending(p => p.CreatedAt)
                    .Select(p => (PaymentStatus?)p.PaymentStatus)
                    .FirstOrDefault()
                };
            }
        }
    }
