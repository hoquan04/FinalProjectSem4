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

            // Nếu truyền Password thì hash
            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                model.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);
                model.Password = null; // clear input
            }
            if (!Enum.IsDefined(typeof(UserRole), model.Role))
            {
                model.Role = UserRole.Customer;
            }

            // Nếu chưa chọn vai trò (ví dụ null hoặc không gửi lên), mặc định là Customer
            // Không được tự động ép sang Admin
            if (!Enum.IsDefined(typeof(UserRole), model.Role))
            {
                model.Role = UserRole.Customer;
            }


            _context.Users.Add(model);
            await _context.SaveChangesAsync();

            // Không trả hash ra ngoài
            model.PasswordHash = string.Empty;

            response.Data = model;
            response.Success = true;
            response.Message = "Thêm người dùng thành công (Admin mặc định)";
            return response;
        }

        public async Task<APIRespone<User>> UpdateUserAsync(int id, User model)
        {
            var response = new APIRespone<User>();
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                response.Success = false;
                response.Message = "Không tìm thấy người dùng";
                return response;
            }

            user.FullName = model.FullName;
            user.Email = model.Email;
            user.Phone = model.Phone;
            user.Address = model.Address;
            //user.PasswordHash = model.PasswordHash;
            //user.Role = model.Role;

            await _context.SaveChangesAsync();
            response.Data = user;
            response.Success = true;
            response.Message = "Cập nhật thành công";
            return response;
        }

        public async Task<APIRespone<bool>> DeleteUserAsync(int id)
        {
            var response = new APIRespone<bool>();
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
            response.Data = true;
            response.Success = true;
            response.Message = "Xóa thành công";
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

        public async Task<APIRespone<PagedResponse<User>>> GetPageAsync(int pageNow, int pageSize)
        {
            var response = new APIRespone<PagedResponse<User>>();
            var totalCount = await _context.Users.CountAsync();
            var totalPage = (int)Math.Ceiling((double)totalCount / pageSize);

            var data = await _context.Users
                .OrderBy(u => u.UserId)
                .Skip((pageNow - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            response.Data = new PagedResponse<User>
            {
                Data = data,
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
        public async Task<APIRespone<User>> ApproveShipperRequestAsync(int userId, bool isApproved)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return new APIRespone<User> { Success = false, Message = "Không tìm thấy người dùng" };

            if (!user.IsShipperRequestPending)
                return new APIRespone<User> { Success = false, Message = "Người này chưa gửi yêu cầu Shipper" };

            if (isApproved)
            {
                user.Role = UserRole.Shipper;
                user.IsShipperRequestPending = false;
                await _context.SaveChangesAsync();

                // ✅ Gửi thông báo cho user
                await _notificationRepo.AddAsync(new Notification
                {
                    UserId = user.UserId,
                    Title = "Yêu cầu Shipper đã được duyệt",
                    Message = "Chúc mừng! Bạn đã được chấp nhận trở thành Shipper.",
                    Type = NotificationType.RoleUpdate,
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                });

                return new APIRespone<User> { Success = true, Data = user, Message = "✅ Đã phê duyệt Shipper" };
            }
            else
            {
                user.IsShipperRequestPending = false;
                user.CccdFrontUrl = null;
                user.CccdBackUrl = null;
                await _context.SaveChangesAsync();

                await _notificationRepo.AddAsync(new Notification
                {
                    UserId = user.UserId,
                    Title = "Yêu cầu Shipper đã được duyệt",
                    Message = "Chúc mừng! Bạn đã được chấp nhận trở thành Shipper.",
                    Type = NotificationType.RoleUpdate, // ✅ đúng kiểu enum
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                });


                return new APIRespone<User> { Success = true, Data = user, Message = "❌ Đã từ chối yêu cầu Shipper" };
            }

        }
    }
}
