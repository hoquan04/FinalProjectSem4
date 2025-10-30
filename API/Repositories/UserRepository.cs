using API.Data;
using API.Models;
using API.Repositories.IRepositories;
using API.Repositories.RestAPI;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        private readonly INotificationRepository _notificationRepo;

        public UserRepository(DataContext context, INotificationRepository notificationRepo)
        {
            _context = context;
            _notificationRepo = notificationRepo;
        }

        public async Task<APIRespone<List<User>>> GetAllUsersAsync()
        {
            var response = new APIRespone<List<User>>();
            response.Data = await _context.Users.ToListAsync();
            response.Success = true;
            return response;
        }

        public async Task<APIRespone<User>> GetUserByIdAsync(int id)
        {
            var response = new APIRespone<User>();
            var user = await _context.Users.FindAsync(id);
            response.Data = user;
            response.Success = user != null;
            response.Message = user == null ? "Không tìm thấy người dùng" : "Thành công";
            return response;
        }

        public async Task<APIRespone<User>> CreateUserAsync(User model)
        {
            var response = new APIRespone<User>();

            // ✅ Bắt buộc có password khi tạo
            if (string.IsNullOrWhiteSpace(model.Password))
                return new APIRespone<User>
                {
                    Success = false,
                    Message = "Password là bắt buộc"
                };

            // (khuyến nghị) chặn trùng email
            if (await _context.Users.AnyAsync(u => u.Email == model.Email))
                return new APIRespone<User>
                {
                    Success = false,
                    Message = "Email đã tồn tại"
                };

            // ✅ Hash
            model.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);
            model.Password = null;

            // ✅ KHÔNG auto nâng quyền; giữ nguyên model.Role gửi từ UI
            // ✅ CreatedAt: có default ở entity (UtcNow), có thể để nguyên



            _context.Users.Add(model);
            await _context.SaveChangesAsync();

            // Không cần set rỗng vì đã JsonIgnore, nhưng giữ cũng không sao
            model.PasswordHash = string.Empty;

            response.Data = model;
            response.Success = true;
            response.Message = "Thêm người dùng thành công";
            return response;
        }

        //public async Task<APIRespone<User>> UpdateUserAsync(int id, User model)
        //{
        //    var user = await _context.Users.FindAsync(id);
        //    if (user == null)
        //        return new APIRespone<User> { Success = false, Message = "Không tìm thấy người dùng" };

        //    // 🧩 Nếu client không gửi role hợp lệ thì giữ nguyên role cũ
        //    if (!Enum.IsDefined(typeof(UserRole), model.Role))
        //        model.Role = user.Role;

        //    user.FullName = model.FullName;
        //    user.Email = model.Email;
        //    user.Phone = model.Phone;
        //    user.Address = model.Address;
        //    user.Role = model.Role;

        //    await _context.SaveChangesAsync();

        //    return new APIRespone<User>
        //    {
        //        Success = true,
        //        Data = user,
        //        Message = "Cập nhật thành công"
        //    };
        //}

        public async Task<APIRespone<User>> UpdateUserAsync(int id, User model)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return new APIRespone<User> { Success = false, Message = "Không tìm thấy người dùng" };

            // 🧩 Giữ nguyên giá trị cũ nếu client không gửi (null hoặc rỗng)
            if (!string.IsNullOrWhiteSpace(model.FullName))
                user.FullName = model.FullName;

            if (!string.IsNullOrWhiteSpace(model.Email))
                user.Email = model.Email;

            if (!string.IsNullOrWhiteSpace(model.Phone))
                user.Phone = model.Phone;

            if (!string.IsNullOrWhiteSpace(model.Address))
                user.Address = model.Address;

            // 🧩 Giữ nguyên Role
            if (Enum.IsDefined(typeof(UserRole), model.Role))
                user.Role = model.Role;

            await _context.SaveChangesAsync();

            return new APIRespone<User>
            {
                Success = true,
                Data = user,
                Message = "Cập nhật thành công"
            };
        }

        public async Task<APIRespone<bool>> DeleteUserAsync(int id)
        {
            var response = new APIRespone<bool>();
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    response.Success = false;
                    response.Data = false;
                    response.Message = "Không tìm thấy người dùng";
                    return response;
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                response.Success = true;
                response.Data = true;
                response.Message = "Xóa người dùng thành công";
            }
            catch (DbUpdateException dbEx)
            {
                // Xảy ra khi có ràng buộc khóa ngoại
                response.Success = false;
                response.Data = false;
                response.Message = "Không thể xóa người dùng vì có dữ liệu liên quan (đơn hàng, thanh toán).";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Data = false;
                response.Message = "Lỗi máy chủ: " + ex.Message;
            }

            return response;
        }


        public async Task<APIRespone<List<User>>> SearchUsersAsync(string searchTerm)
        {
            var response = new APIRespone<List<User>>();
            response.Data = await _context.Users
                .Where(u => u.FullName.Contains(searchTerm) || u.Email.Contains(searchTerm))
                .ToListAsync();
            response.Success = true;
            response.Message = "Tìm kiếm thành công";
            return response;
        }

        public async Task<APIRespone<PagedResponse<User>>> GetPageAsync(int pageNow, int pageSize, string? search = null)
        {
            if (pageNow <= 0) pageNow = 1;
            if (pageSize <= 0) pageSize = 10;

            var response = new APIRespone<PagedResponse<User>>();

            IQueryable<User> query = _context.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var keyword = search.Trim();
                query = query.Where(u =>
                    u.FullName.Contains(keyword) ||
                    u.Email.Contains(keyword) ||
                    u.Phone.Contains(keyword));
            }

            var totalCount = await query.CountAsync();
            var totalPage = (int)Math.Ceiling((double)totalCount / pageSize);

            var data = await query
                .OrderBy(u => u.UserId)
                .Skip((pageNow - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            response.Data = new PagedResponse<User>
            {
                Data = data,                  // IEnumerable<User>
                PageNow = pageNow,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPage = totalPage
            };
            response.Success = true;
            return response;
        }



        public async Task<APIRespone<User>> UpgradeToShipperAsync(int userId, string cccdImageUrl)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return new APIRespone<User> { Success = false, Message = "Không tìm thấy user" };

            user.Role = UserRole.Shipper;
            // bạn có thể lưu thêm thông tin cccdImageUrl nếu muốn
            await _context.SaveChangesAsync();

            return new APIRespone<User> { Success = true, Data = user, Message = "Đã cập nhật thành Shipper" };
        }
        public async Task<APIRespone<User>> RequestShipperAsync(int userId, string cccdFrontUrl, string cccdBackUrl)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return new APIRespone<User> { Success = false, Message = "Không tìm thấy người dùng" };

            // 🚫 Nếu đã là Shipper thì không cho gửi yêu cầu nữa
            if (user.Role == UserRole.Shipper)
                return new APIRespone<User> { Success = false, Message = "Tài khoản đã là shipper" };

            // ✅ Ghi lại thông tin ảnh và trạng thái chờ duyệt
            user.CccdFrontUrl = cccdFrontUrl;
            user.CccdBackUrl = cccdBackUrl;
            user.IsShipperRequestPending = true;

            // 🚫 KHÔNG thay đổi Role ở đây — vẫn giữ là Customer
            await _context.SaveChangesAsync();

            return new APIRespone<User>
            {
                Success = true,
                Data = user,
                Message = "Đã gửi yêu cầu đăng ký Shipper, vui lòng chờ admin xét duyệt"
            };
        }

        //public async Task<APIRespone<User>> ApproveShipperRequestAsync(int userId, bool isApproved)
        //{
        //    var user = await _context.Users.FindAsync(userId);
        //    if (user == null)
        //        return new APIRespone<User> { Success = false, Message = "Không tìm thấy người dùng" };

        //    if (!user.IsShipperRequestPending)
        //        return new APIRespone<User> { Success = false, Message = "Người này chưa gửi yêu cầu Shipper" };

        //    if (isApproved)
        //    {
        //        user.Role = UserRole.Shipper;
        //        user.IsShipperRequestPending = false;
        //        await _context.SaveChangesAsync();
        //        return new APIRespone<User> { Success = true, Data = user, Message = "✅ Đã phê duyệt Shipper" };
        //    }
        //    else
        //    {
        //        user.IsShipperRequestPending = false;
        //        user.CccdFrontUrl = null;
        //        user.CccdBackUrl = null;
        //        await _context.SaveChangesAsync();
        //        return new APIRespone<User> { Success = true, Data = user, Message = "❌ Đã từ chối yêu cầu Shipper" };
        //    }
        //}
        //public async Task<APIRespone<User>> ApproveShipperRequestAsync(int userId, bool isApproved)
        //{
        //    var user = await _context.Users.FindAsync(userId);
        //    if (user == null)
        //        return new APIRespone<User> { Success = false, Message = "Không tìm thấy người dùng" };

        //    if (!user.IsShipperRequestPending)
        //        return new APIRespone<User> { Success = false, Message = "Người này chưa gửi yêu cầu Shipper" };

        //    if (isApproved)
        //    {
        //        user.Role = UserRole.Shipper;
        //        user.IsShipperRequestPending = false;
        //        await _context.SaveChangesAsync();

        //        // ✅ Gửi thông báo cho user
        //        await _notificationRepo.AddAsync(new Notification
        //        {
        //            UserId = user.UserId,
        //            Title = "Yêu cầu Shipper đã được duyệt",
        //            Message = "Chúc mừng! Bạn đã được chấp nhận trở thành Shipper.",
        //            Type = NotificationType.RoleUpdate,
        //            IsRead = false,
        //            CreatedAt = DateTime.UtcNow
        //        });

        //        return new APIRespone<User> { Success = true, Data = user, Message = "✅ Đã phê duyệt Shipper" };
        //    }
        //    else
        //    {
        //        user.IsShipperRequestPending = false;
        //        user.CccdFrontUrl = null;
        //        user.CccdBackUrl = null;
        //        await _context.SaveChangesAsync();

        //        await _notificationRepo.AddAsync(new Notification
        //        {
        //            UserId = user.UserId,
        //            Title = "Yêu cầu Shipper đã được duyệt",
        //            Message = "Chúc mừng! Bạn đã được chấp nhận trở thành Shipper.",
        //            Type = NotificationType.RoleUpdate, // ✅ đúng kiểu enum
        //            IsRead = false,
        //            CreatedAt = DateTime.UtcNow
        //        });


        //        return new APIRespone<User> { Success = true, Data = user, Message = "❌ Đã từ chối yêu cầu Shipper" };
        //    }

        //}
        public async Task<APIRespone<User>> ApproveShipperRequestAsync(int userId, bool isApproved)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return new APIRespone<User> { Success = false, Message = "Không tìm thấy người dùng" };

            if (!user.IsShipperRequestPending)
                return new APIRespone<User> { Success = false, Message = "Người này chưa gửi yêu cầu Shipper" };

            if (isApproved)
            {
                // ✅ Duyệt
                user.Role = UserRole.Shipper;
                user.IsShipperRequestPending = false;
                await _context.SaveChangesAsync();

                await _notificationRepo.AddAsync(new Notification
                {
                    UserId = user.UserId,
                    Title = "Yêu cầu Shipper đã được duyệt",
                    Message = "Chúc mừng! Bạn đã được chấp nhận trở thành Shipper của cửa hàng.",
                    Type = NotificationType.RoleUpdate,
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                });

                return new APIRespone<User>
                {
                    Success = true,
                    Data = user,
                    Message = "✅ Đã phê duyệt Shipper"
                };
            }
            else
            {
                // ❌ Từ chối
                user.IsShipperRequestPending = false;
                user.CccdFrontUrl = null;
                user.CccdBackUrl = null;
                await _context.SaveChangesAsync();

                await _notificationRepo.AddAsync(new Notification
                {
                    UserId = user.UserId,
                    Title = "Yêu cầu Shipper bị từ chối",
                    Message = "Rất tiếc! Yêu cầu đăng ký Shipper của bạn đã bị từ chối. Vui lòng kiểm tra lại thông tin hoặc liên hệ quản trị viên để biết thêm chi tiết.",
                    Type = NotificationType.RoleUpdate,
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                });

                return new APIRespone<User>
                {
                    Success = true,
                    Data = user,
                    Message = "❌ Đã từ chối yêu cầu Shipper"
                };
            }
        }

    }
}
