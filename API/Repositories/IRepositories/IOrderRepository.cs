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

        Task<APIRespone<IEnumerable<ShipperOrderDto>>> GetAvailableOrdersForShipperAsync();

        Task<APIRespone<Order>> AssignOrderToShipperAsync(int orderId, int shipperId);
        Task<APIRespone<IEnumerable<object>>> GetOrdersOfShipperAsync(int shipperId);
        Task<APIRespone<Order>> CompleteOrderByShipperAsync(int orderId, int shipperId);
        Task<APIRespone<PagedResponse<OrderDisplayDto>>> GetAllAsync(int pageNow = 1, int pageSize = 10);
        Task<APIRespone<PagedResponse<OrderDisplayDto>>> Searchdto(int pageNow, int pageSize, SearchOrder search);


    }
}
