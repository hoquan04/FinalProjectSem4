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

            // Nếu truyền Password thì hash
            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                model.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);
                model.Password = null; // clear input
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
    }
}
