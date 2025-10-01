using API.Models.DTOs;
using API.Repositories.IRepositories;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly ICartRepository _repo;
        public CartController(ICartRepository repo)
        {
            _repo = repo;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetCart(int userId)
        {
            var res = await _repo.GetCartByUserAsync(userId);
            return Ok(res);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddToCart(int userId, int productId, int quantity = 1)
        {
            var res = await _repo.AddToCartAsync(userId, productId, quantity);
            return Ok(res);
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateQuantity(int cartId, int quantity)
        {
            var res = await _repo.UpdateQuantityAsync(cartId, quantity);
            return Ok(res);
        }

        [HttpDelete("remove-multi")]
        public async Task<IActionResult> RemoveItems([FromBody] List<int> cartIds)
        {
            var res = await _repo.RemoveItemsAsync(cartIds);
            return Ok(res);
        }


        [HttpDelete("clear/{userId}")]
        public async Task<IActionResult> ClearCart(int userId)
        {
            var res = await _repo.ClearCartAsync(userId);
            return Ok(res);
        }
    }
}
