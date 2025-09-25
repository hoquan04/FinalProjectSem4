using API.Models;
using API.Models.DTOs;
using API.Repositories.RestAPI;

namespace API.Repositories.IRepositories
{
    public interface IShippingRepository : IType<Shipping>
    {
        Task<APIRespone<IEnumerable<Shipping>>> SearchAsync(string? address, string? city);
        Task<APIRespone<PagedResponse<Shipping>>> SearchWithPaginationAsync(string? address, string? city, int pageNow, int pageSize);

    }
}
