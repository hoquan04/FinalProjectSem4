
using API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Repositories.IRepositories
{
    public interface IUserRepository
    {
        Task<ApiResponse<List<User>>> GetAllUsersAsync();
        Task<ApiResponse<User>> GetUserByIdAsync(int id);
        Task<ApiResponse<User>> CreateUserAsync(User model);
        Task<ApiResponse<User>> UpdateUserAsync(int id, User model);
        Task<ApiResponse<bool>> DeleteUserAsync(int id);
        Task<ApiResponse<List<User>>> SearchUsersAsync(string searchTerm);
        Task<ApiResponse<PagedResponse<User>>> GetPageAsync(int pageNow, int pageSize);
    }
}
