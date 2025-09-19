
using API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.Repositories.RestAPI;
namespace API.Repositories.IRepositories
{
    public interface IUserRepository
    {
        Task<APIRespone<List<User>>> GetAllUsersAsync();
        Task<APIRespone<User>> GetUserByIdAsync(int id);
        Task<APIRespone<User>> CreateUserAsync(User model);
        Task<APIRespone<User>> UpdateUserAsync(int id, User model);
        Task<APIRespone<bool>> DeleteUserAsync(int id);
        Task<APIRespone<List<User>>> SearchUsersAsync(string searchTerm);
        Task<APIRespone<PagedResponse<User>>> GetPageAsync(int pageNow, int pageSize);
    }
}
