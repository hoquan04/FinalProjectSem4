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

        public UserRepository(DataContext context)
        {
            _context = context;
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

        public async Task<APIRespone<User>> UpdateUserAsync(int id, User model)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return new APIRespone<User> { Success = false, Message = "Không tìm thấy người dùng" };

            user.FullName = model.FullName;
            user.Email = model.Email;
            user.Phone = model.Phone;
            user.Address = model.Address;
            user.Role = model.Role;   // ✅ Cho phép đổi quyền từ UI admin

            await _context.SaveChangesAsync();
            return new APIRespone<User> { Success = true, Data = user, Message = "Cập nhật thành công" };
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
    }
}
