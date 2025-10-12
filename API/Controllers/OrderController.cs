using API.Data;
using API.Models;
using API.Models.DTOs;
using API.Repositories.IRepositories;
using API.Repositories.RestAPI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly DataContext _context; // ✅ thêm dòng này

        public OrderController(IOrderRepository orderRepository, DataContext context)
        {
            _orderRepository = orderRepository;
            _context = context; // ✅ gán context
        }


        // ✅ GET: api/Order?pageNow=1&pageSize=10
        [HttpGet]
        public async Task<ActionResult<APIRespone<PagedResponse<OrderDisplayDto>>>> GetAll(
            [FromQuery] int pageNow = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _orderRepository.GetAllAsync(pageNow, pageSize);
            return Ok(result);
        }

        // ✅ GET: api/Order/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<APIRespone<Order>>> GetById(int id)
        {
            var result = await _orderRepository.GetByIdAsync(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        // ✅ POST: api/Order
        [HttpPost]
        public async Task<ActionResult<APIRespone<Order>>> Add([FromBody] Order order)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _orderRepository.AddAsync(order);
            return Ok(result);
        }

        // ✅ PUT: api/Order/{id}
        [HttpPut("{id:int}")]
        public async Task<ActionResult<APIRespone<Order>>> Update(int id, [FromBody] Order order)
        {
            if (id != order.OrderId)
                return BadRequest("ID không khớp với đối tượng cập nhật.");

            var result = await _orderRepository.UpdateAsync(id, order);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        // ✅ DELETE: api/Order/{id}
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<APIRespone<bool>>> Delete(int id)
        {
            var result = await _orderRepository.DeleteAsync(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        // ✅ POST: api/Order/searchdto?pageNow=1&pageSize=10
        [HttpPost("searchdto")]
        public async Task<ActionResult<APIRespone<PagedResponse<OrderDisplayDto>>>> Searchdto(
            [FromBody] SearchOrder search,
            [FromQuery] int pageNow = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _orderRepository.Searchdto(pageNow, pageSize, search);
            return Ok(result);
        }

        // ✅ GET: api/Order/user/{userId}
        [HttpGet("user/{userId:int}")]
        public async Task<ActionResult<APIRespone<IEnumerable<Order>>>> GetByUserId(int userId)
        {
            var result = await _orderRepository.GetByUserIdAsync(userId);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        // ✅ GET: api/Order/shipper
        [HttpGet("shipper")]
        public async Task<ActionResult<APIRespone<IEnumerable<Order>>>> GetOrdersForShipper()
        {
            var result = await _orderRepository.GetOrdersForShipperAsync();
            return Ok(result);
        }



        // ✅ PUT: api/Order/shipper/assign?orderId=1&shipperId=2
        [HttpPut("shipper/assign")]
        public async Task<ActionResult<APIRespone<Order>>> AssignOrderToShipper(
            [FromQuery] int orderId,
            [FromQuery] int shipperId)
        {
            var result = await _orderRepository.AssignOrderToShipperAsync(orderId, shipperId);
            return Ok(result);
        }

        // ✅ GET: api/Order/shipper/{shipperId}/orders
        [HttpGet("shipper/{shipperId:int}/orders")]
        public async Task<ActionResult<APIRespone<IEnumerable<object>>>> GetOrdersOfShipper(int shipperId)
        {
            var result = await _orderRepository.GetOrdersOfShipperAsync(shipperId);
            return Ok(result);
        }

        // ✅ PUT: api/Order/shipper/{shipperId}/complete/{orderId}
        [HttpPut("shipper/{shipperId:int}/complete/{orderId:int}")]
        public async Task<ActionResult<APIRespone<Order>>> CompleteOrderByShipper(int orderId, int shipperId)
        {
            var result = await _orderRepository.CompleteOrderByShipperAsync(orderId, shipperId);
            return Ok(result);
        }
        // ✅ GET: api/Order/history/{userId}
        [HttpGet("history/{userId:int}")]
        public async Task<ActionResult<APIRespone<IEnumerable<object>>>> GetOrderHistoryByUser(int userId)
        {
            var orders = await _context.Orders
                .Include(o => o.Shipping)
                .Include(o => o.OrderDetails).ThenInclude(od => od.Product)
                .Include(o => o.Payments)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .Select(o => new
                {
                    order = o,
                    shipping = o.Shipping,
                    orderDetails = o.OrderDetails,
                    payments = o.Payments
                })
                .ToListAsync();

            return new APIRespone<IEnumerable<object>>
            {
                Success = true,
                Message = "Lấy lịch sử đơn hàng đầy đủ thành công",
                Data = orders
            };
        }

    }
}
