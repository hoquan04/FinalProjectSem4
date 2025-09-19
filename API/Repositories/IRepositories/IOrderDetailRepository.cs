using API.Models;
using API.Models.DTOs;
using API.Repositories.RestAPI;

namespace API.Repositories.IRepositories
{
    public interface IOrderDetailRepository
    {
        Task<APIRespone<IEnumerable<OrderDetailDto>>> GetAllAsync();
        Task<APIRespone<OrderDetailDto>> GetByIdAsync(int id);
        Task<APIRespone<OrderDetailDto>> AddAsync(OrderDetail entity);
        Task<APIRespone<OrderDetailDto>> UpdateAsync(int id, OrderDetail entity);
        Task<APIRespone<bool>> DeleteAsync(int id);
        Task<APIRespone<PagedResponse<OrderDetailDto>>> GetPageAsync(int pageNow, int pageSize);
        Task<APIRespone<PagedResponse<OrderDetailDto>>> Search(int pageNow, int pageSize, SearchOrderDetail search);
    }

}
