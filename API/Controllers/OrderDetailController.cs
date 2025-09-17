using API.Models;
using API.Models.DTOs;
using API.Repositories.IRepositories;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderDetailController : ControllerBase
    {
        private readonly IOrderDetailRepository _orderDetailRepo;

        public OrderDetailController(IOrderDetailRepository orderDetailRepo)
        {
            _orderDetailRepo = orderDetailRepo;
        }

        // Lấy tất cả chi tiết đơn hàng
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _orderDetailRepo.GetAllAsync();
            return Ok(result);
        }

        // Lấy chi tiết đơn hàng theo Id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _orderDetailRepo.GetByIdAsync(id);
            return Ok(result);
        }

        // Thêm chi tiết đơn hàng
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] OrderDetail model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _orderDetailRepo.AddAsync(model);
            return Ok(result);
        }

        // Cập nhật chi tiết đơn hàng
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] OrderDetail model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _orderDetailRepo.UpdateAsync(id, model);
            return Ok(result);
        }

        // Xóa chi tiết đơn hàng
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _orderDetailRepo.DeleteAsync(id);
            return Ok(result);
        }

        // Lấy chi tiết đơn hàng có phân trang
        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged([FromQuery] int pageNow = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _orderDetailRepo.GetPageAsync(pageNow, pageSize);
            return Ok(result);
        }

        // Tìm kiếm chi tiết đơn hàng
        [HttpPost("search")]
        public async Task<IActionResult> Search(
        [FromBody] SearchOrderDetail search,
        [FromQuery] int pageNow = 1,
        [FromQuery] int pageSize = 10)
        {
            var result = await _orderDetailRepo.Search(pageNow, pageSize, search);
            return Ok(result);
        }

    }
}
