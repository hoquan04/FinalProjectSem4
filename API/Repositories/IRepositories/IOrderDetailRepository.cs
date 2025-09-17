using API.Models;
using API.Models.DTOs;
using API.Repositories.RestAPI;

namespace API.Repositories.IRepositories
{
    public interface IOrderDetailRepository : IType<OrderDetail>
    {
        Task<APIRespone<PagedResponse<OrderDetail>>> Search(int pageNow, int pageSize, SearchOrderDetail search);
    }
}
