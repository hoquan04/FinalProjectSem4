using API.Models;
using API.Repositories.RestAPI;

namespace API.Repositories.IRepositories
{
    public interface IReviewRepository : IType<Review>
    {
        Task<APIRespone<IEnumerable<Review>>> GetByProductIdAsync(int productId);
        Task<APIRespone<IEnumerable<Review>>> GetByUserIdAsync(int userId);
        Task<APIRespone<double>> GetAverageRatingByProductIdAsync(int productId);

        Task<APIRespone<PagedResponse<Review>>> SearchAsync(string? searchKeyword, int pageNow, int pageSize);
    }
}
