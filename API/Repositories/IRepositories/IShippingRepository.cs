using API.Models;
using API.Models.DTOs;
using API.Repositories.RestAPI;

namespace API.Repositories.IRepositories
{
    public interface IShippingRepository : IType<Shipping>
    {
            Task<APIRespone<IEnumerable<Shipping>>> SearchAsync(string? recipientName, string? phoneNumber, string? address, string? city, string? postalCode);

        Task<APIRespone<PagedResponse<Shipping>>> SearchWithPaginationAsync(
            string? recipientName, string? phoneNumber, string? address, string? city, string? postalCode,
            int pageNow, int pageSize);

    }
}
