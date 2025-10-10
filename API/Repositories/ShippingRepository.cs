using API.Data;
using API.Models;
using API.Models.DTOs;
using API.Repositories.IRepositories;
using API.Repositories.RestAPI;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace API.Repositories
{
    public class ShippingRepository : IShippingRepository
    {
        private readonly DataContext _context;

        public ShippingRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<APIRespone<IEnumerable<Shipping>>> GetAllAsync()
        {
            try
            {
                var shippings = await _context.Shippings
                 .Select(s => new Shipping
                 {
                     ShippingId = s.ShippingId,
                     RecipientName = s.RecipientName,
                     PhoneNumber = s.PhoneNumber,
                     Email = s.Email,
                     Address = s.Address,
                     City = s.City,
                     PostalCode = s.PostalCode,
                     ShippingFee = s.ShippingFee,
                     EstimatedDays = s.EstimatedDays,
                     CreatedAt = s.CreatedAt
                 })
                .ToListAsync();


                return new APIRespone<IEnumerable<Shipping>>
                {
                    Success = true,
                    Message = "Lấy danh sách thành công",
                    Data = shippings
                };
            }
            catch (Exception ex)
            {
                return new APIRespone<IEnumerable<Shipping>>
                {
                    Success = false,
                    Message = $"Lỗi: {ex.Message}",
                    Data = null
                };
            }
        }

        public async Task<APIRespone<Shipping>> GetByIdAsync(int id)
        {
            try
            {
                var shipping = await _context.Shippings
                    .Where(s => s.ShippingId == id)
                    .Select(s => new Shipping
                    {
                        ShippingId = s.ShippingId,
                        RecipientName = s.RecipientName,
                        PhoneNumber = s.PhoneNumber,
                        Email = s.Email,
                        Address = s.Address,
                        City = s.City,
                        PostalCode = s.PostalCode,
                        ShippingFee = s.ShippingFee,
                        EstimatedDays = s.EstimatedDays,
                        CreatedAt = s.CreatedAt
                    })
                    .FirstOrDefaultAsync();

                if (shipping == null)
                {
                    return new APIRespone<Shipping>
                    {
                        Success = false,
                        Message = "Không tìm thấy thông tin vận chuyển",
                        Data = null
                    };
                }

                return new APIRespone<Shipping>
                {
                    Success = true,
                    Message = "Lấy thông tin thành công",
                    Data = shipping
                };
            }
            catch (Exception ex)
            {
                return new APIRespone<Shipping>
                {
                    Success = false,
                    Message = $"Lỗi: {ex.Message}",
                    Data = null
                };
            }
        }


        public async Task<APIRespone<Shipping>> AddAsync(Shipping entity)
        {
            try
            {
                await _context.Shippings.AddAsync(entity);
                await _context.SaveChangesAsync();

                // Trả về entity đã tạo mà không có navigation properties
                var createdShipping = new Shipping
                {
                    ShippingId = entity.ShippingId,
                    RecipientName = entity.RecipientName,
                    PhoneNumber = entity.PhoneNumber,
                    Email = entity.Email,
                    Address = entity.Address,
                    City = entity.City,
                    PostalCode = entity.PostalCode,
                    ShippingFee = entity.ShippingFee,
                    EstimatedDays = entity.EstimatedDays,
                    CreatedAt = entity.CreatedAt
                };


                return new APIRespone<Shipping>
                {
                    Success = true,
                    Message = "Thêm thông tin vận chuyển thành công",
                    Data = createdShipping
                };
            }
            catch (Exception ex)
            {
                return new APIRespone<Shipping>
                {
                    Success = false,
                    Message = $"Lỗi: {ex.Message}",
                    Data = null
                };
            }
        }

        public async Task<APIRespone<Shipping>> UpdateAsync(int id, Shipping entity)
        {
            try
            {
                var existingShipping = await _context.Shippings.FindAsync(id);
                if (existingShipping == null)
                {
                    return new APIRespone<Shipping>
                    {
                        Success = false,
                        Message = "Không tìm thấy thông tin vận chuyển",
                        Data = null
                    };
                }
                existingShipping.RecipientName = entity.RecipientName;
                existingShipping.PhoneNumber = entity.PhoneNumber;
                existingShipping.Email = entity.Email;
                existingShipping.Address = entity.Address;
                existingShipping.City = entity.City;
                existingShipping.PostalCode = entity.PostalCode;
                existingShipping.ShippingFee = entity.ShippingFee;
                existingShipping.EstimatedDays = entity.EstimatedDays;


                await _context.SaveChangesAsync();

                // Trả về entity đã cập nhật mà không có navigation properties
                var updatedShipping = new Shipping
                {
                    ShippingId = existingShipping.ShippingId,
                    Address = existingShipping.Address,
                    City = existingShipping.City,
                    PostalCode = existingShipping.PostalCode,
                    ShippingFee = existingShipping.ShippingFee,
                    EstimatedDays = existingShipping.EstimatedDays
                };

                return new APIRespone<Shipping>
                {
                    Success = true,
                    Message = "Cập nhật thành công",
                    Data = updatedShipping
                };
            }
            catch (Exception ex)
            {
                return new APIRespone<Shipping>
                {
                    Success = false,
                    Message = $"Lỗi: {ex.Message}",
                    Data = null
                };
            }
        }

        public async Task<APIRespone<bool>> DeleteAsync(int id)
        {
            try
            {
                var shipping = await _context.Shippings.FindAsync(id);
                if (shipping == null)
                {
                    return new APIRespone<bool>
                    {
                        Success = false,
                        Message = "Không tìm thấy thông tin vận chuyển",
                        Data = false
                    };
                }

                // Kiểm tra xem có đơn hàng nào đang sử dụng shipping này không
                var hasOrders = await _context.Orders.AnyAsync(o => o.ShippingId == id);
                if (hasOrders)
                {
                    return new APIRespone<bool>
                    {
                        Success = false,
                        Message = "Không thể xóa vì có đơn hàng đang sử dụng thông tin vận chuyển này",
                        Data = false
                    };
                }

                _context.Shippings.Remove(shipping);
                await _context.SaveChangesAsync();

                return new APIRespone<bool>
                {
                    Success = true,
                    Message = "Xóa thành công",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                return new APIRespone<bool>
                {
                    Success = false,
                    Message = $"Lỗi: {ex.Message}",
                    Data = false
                };
            }
        }

        public async Task<APIRespone<PagedResponse<Shipping>>> GetPageAsync(int pageNow, int pageSize)
        {
            try
            {
                var query = _context.Shippings.AsQueryable();

                var totalCount = await query.CountAsync();
                var totalPage = (int)Math.Ceiling((double)totalCount / pageSize);

                var shippings = await query
                    .OrderByDescending(s => s.CreatedAt) // mới nhất lên trước
                    .Skip((pageNow - 1) * pageSize)
                    .Take(pageSize)
                    .Select(s => new Shipping
                    {
                        ShippingId = s.ShippingId,
                        RecipientName = s.RecipientName,
                        PhoneNumber = s.PhoneNumber,
                        Email = s.Email,
                        Address = s.Address,
                        City = s.City,
                        PostalCode = s.PostalCode,
                        ShippingFee = s.ShippingFee,
                        EstimatedDays = s.EstimatedDays,
                        CreatedAt = s.CreatedAt
                    })
                    .ToListAsync();

                var pagedResponse = new PagedResponse<Shipping>
                {
                    Data = shippings,
                    PageNow = pageNow,
                    PageSize = pageSize,
                    TotalPage = totalPage,
                    TotalCount = totalCount
                };

                return new APIRespone<PagedResponse<Shipping>>
                {
                    Success = true,
                    Message = "Lấy dữ liệu phân trang thành công",
                    Data = pagedResponse
                };
            }
            catch (Exception ex)
            {
                return new APIRespone<PagedResponse<Shipping>>
                {
                    Success = false,
                    Message = $"Lỗi: {ex.Message}",
                    Data = null
                };
            }
        }


        public async Task<APIRespone<IEnumerable<Shipping>>> SearchAsync(
     string? recipientName, string? phoneNumber, string? address, string? city, string? postalCode)
        {
            try
            {
                var query = _context.Shippings.AsQueryable();

                if (!string.IsNullOrEmpty(recipientName))
                    query = query.Where(s => s.RecipientName.Contains(recipientName));

                if (!string.IsNullOrEmpty(phoneNumber))
                    query = query.Where(s => s.PhoneNumber.Contains(phoneNumber));

                if (!string.IsNullOrEmpty(address))
                    query = query.Where(s => s.Address.Contains(address));

                if (!string.IsNullOrEmpty(city))
                    query = query.Where(s => s.City != null && s.City.Contains(city));

                if (!string.IsNullOrEmpty(postalCode))
                    query = query.Where(s => s.PostalCode != null && s.PostalCode.Contains(postalCode));

                var shippings = await query
                    .OrderByDescending(s => s.CreatedAt)
                    .Select(s => new Shipping
                    {
                        ShippingId = s.ShippingId,
                        RecipientName = s.RecipientName,
                        PhoneNumber = s.PhoneNumber,
                        Email = s.Email,
                        Address = s.Address,
                        City = s.City,
                        PostalCode = s.PostalCode,
                        ShippingFee = s.ShippingFee,
                        EstimatedDays = s.EstimatedDays,
                        CreatedAt = s.CreatedAt
                    })
                    .ToListAsync();

                return new APIRespone<IEnumerable<Shipping>>
                {
                    Success = true,
                    Message = "Tìm kiếm thành công",
                    Data = shippings
                };
            }
            catch (Exception ex)
            {
                return new APIRespone<IEnumerable<Shipping>>
                {
                    Success = false,
                    Message = $"Lỗi: {ex.Message}",
                    Data = null
                };
            }
        }


        public async Task<APIRespone<PagedResponse<Shipping>>> SearchWithPaginationAsync(
     string? recipientName, string? phoneNumber, string? address, string? city, string? postalCode,
     int pageNow, int pageSize)
        {
            try
            {
                var query = _context.Shippings.AsQueryable();

                // 🔍 Các điều kiện tìm kiếm
                if (!string.IsNullOrEmpty(recipientName))
                    query = query.Where(s => s.RecipientName.Contains(recipientName));

                if (!string.IsNullOrEmpty(phoneNumber))
                    query = query.Where(s => s.PhoneNumber.Contains(phoneNumber));

                if (!string.IsNullOrEmpty(address))
                    query = query.Where(s => s.Address.Contains(address));

                if (!string.IsNullOrEmpty(city))
                    query = query.Where(s => s.City != null && s.City.Contains(city));

                if (!string.IsNullOrEmpty(postalCode))
                    query = query.Where(s => s.PostalCode != null && s.PostalCode.Contains(postalCode));

                // 📊 Đếm tổng số bản ghi
                var totalCount = await query.CountAsync();
                var totalPage = (int)Math.Ceiling((double)totalCount / pageSize);

                // ⏩ Lấy dữ liệu phân trang
                var shippings = await query
                    .OrderByDescending(s => s.CreatedAt)
                    .Skip((pageNow - 1) * pageSize)
                    .Take(pageSize)
                    .Select(s => new Shipping
                    {
                        ShippingId = s.ShippingId,
                        RecipientName = s.RecipientName,
                        PhoneNumber = s.PhoneNumber,
                        Email = s.Email,
                        Address = s.Address,
                        City = s.City,
                        PostalCode = s.PostalCode,
                        ShippingFee = s.ShippingFee,
                        EstimatedDays = s.EstimatedDays,
                        CreatedAt = s.CreatedAt
                    })
                    .ToListAsync();

                var pagedResponse = new PagedResponse<Shipping>
                {
                    Data = shippings,
                    PageNow = pageNow,
                    PageSize = pageSize,
                    TotalPage = totalPage,
                    TotalCount = totalCount
                };

                return new APIRespone<PagedResponse<Shipping>>
                {
                    Success = true,
                    Message = "Tìm kiếm với phân trang thành công",
                    Data = pagedResponse
                };
            }
            catch (Exception ex)
            {
                return new APIRespone<PagedResponse<Shipping>>
                {
                    Success = false,
                    Message = $"Lỗi: {ex.Message}",
                    Data = null
                };
            }
        }

    }
}
