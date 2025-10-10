using API.Models;
using API.Models.DTOs;
using API.Repositories.RestAPI;
namespace API.Repositories.IRepositories
{
    public interface IOrderRepository : IType<Order>
    {
        Task<APIRespone<PagedResponse<Order>>> Search(int pageNow, int pageSize, SearchOrder search);
        Task<APIRespone<IEnumerable<Order>>> GetByUserIdAsync(int userId);
        Task<APIRespone<IEnumerable<Order>>> GetOrdersForShipperAsync();

        Task<APIRespone<Order>> CompleteOrderByShipperAsync(int orderId);
        Task<APIRespone<IEnumerable<Order>>> GetAvailableOrdersForShipperAsync();

        Task<APIRespone<Order>> AssignOrderToShipperAsync(int orderId, int shipperId);
        Task<APIRespone<IEnumerable<object>>> GetOrdersOfShipperAsync(int shipperId);
        Task<APIRespone<Order>> CompleteOrderByShipperAsync(int orderId, int shipperId);

    }
}
