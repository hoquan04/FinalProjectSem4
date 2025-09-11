using API.Models;
using API.Repositories.RestAPI;

namespace API.Repositories.IRepositories
{
    public interface IProductRepository : IType<Product>
    {
        // Thêm các method đặc biệt cho Product
        Task<APIRespone<IEnumerable<Product>>> GetProductsByCategoryAsync(int categoryId);
        Task<APIRespone<IEnumerable<Product>>> SearchProductsAsync(string searchTerm);
    }
}
