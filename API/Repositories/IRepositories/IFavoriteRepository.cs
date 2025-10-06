using API.Models;
using API.Repositories.RestAPI;

namespace API.Repositories.IRepositories
{
    public interface IFavoriteRepository
    {
        Task<APIRespone<bool>> AddToFavoritesAsync(int userId, int productId);
        Task<APIRespone<bool>> RemoveFromFavoritesAsync(int userId, int productId);
        Task<APIRespone<bool>> CheckIsFavoriteAsync(int userId, int productId);
        Task<APIRespone<IEnumerable<Favorite>>> GetUserFavoritesAsync(int userId);
        Task<APIRespone<PagedResponse<Favorite>>> GetUserFavoritesPagedAsync(int userId, int pageNow, int pageSize);
        Task<APIRespone<int>> GetFavoriteCountByProductIdAsync(int productId);
        Task<APIRespone<bool>> ClearAllFavoritesAsync(int userId);
        Task<APIRespone<IEnumerable<Favorite>>> GetMostFavoriteProductsAsync(int count = 10);
    }
}
