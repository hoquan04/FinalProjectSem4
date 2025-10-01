using API.Models;
using API.Models.DTOs;
using API.Repositories.RestAPI;

namespace API.Repositories.IRepositories
{
    public interface ICartRepository
    {
        Task<APIRespone<IEnumerable<CartDto>>> GetCartByUserAsync(int userId);
        Task<APIRespone<CartDto>> AddToCartAsync(int userId, int productId, int quantity);
        Task<APIRespone<CartDto>> UpdateQuantityAsync(int cartId, int quantity);
        Task<APIRespone<bool>> RemoveItemsAsync(List<int> cartIds);

        Task<APIRespone<bool>> ClearCartAsync(int userId);
    }
}
