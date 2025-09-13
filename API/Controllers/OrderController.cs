using API.Models;
using API.Models.DTOs;
using API.Repositories.IRepositories;
using API.Repositories.RestAPI;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;

        public OrderController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        // GET: api/Order
        [HttpGet]
        public async Task<ActionResult<APIRespone<IEnumerable<Order>>>> GetAll()
        {
            var result = await _orderRepository.GetAllAsync();
            return Ok(result);
        }

        // GET: api/Order/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<APIRespone<Order>>> GetById(int id)
        {
            var result = await _orderRepository.GetByIdAsync(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        // POST: api/Order
        [HttpPost]
        public async Task<ActionResult<APIRespone<Order>>> Add(Order order)
        {
            var result = await _orderRepository.AddAsync(order);
            return Ok(result);
        }

        // PUT: api/Order/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<APIRespone<Order>>> Update(int id, Order order)
        {
            var result = await _orderRepository.UpdateAsync(id, order);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        // DELETE: api/Order/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult<APIRespone<bool>>> Delete(int id)
        {
            var result = await _orderRepository.DeleteAsync(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        // GET: api/Order/page?pageNow=1&pageSize=10
        [HttpGet("page")]
        public async Task<ActionResult<APIRespone<PagedResponse<Order>>>> GetPage(int pageNow = 1, int pageSize = 10)
        {
            var result = await _orderRepository.GetPageAsync(pageNow, pageSize);
            return Ok(result);
        }

        // POST: api/Order/search?pageNow=1&pageSize=10
        [HttpPost("search")]
        public async Task<ActionResult<APIRespone<PagedResponse<Order>>>> Search(
            [FromBody] SearchOrder search,
            int pageNow = 1,
            int pageSize = 10
        )

        {
            var result = await _orderRepository.Search(pageNow, pageSize, search);
            return Ok(result);
        }
    }
}
