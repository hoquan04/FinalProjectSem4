using API.Models;
using API.Models.DTOs;
using API.Repositories.RestAPI;
namespace API.Repositories.IRepositories
{
    public interface IOrderRepository : IType<Order>
    {
        Task<APIRespone<PagedResponse<Order>>> Search(int pageNow, int pageSize, SearchOrder search);

    }
}
