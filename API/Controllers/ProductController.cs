using API.Models;
using API.Repositories.IRepositories;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly IFavoriteRepository _favoriteRepository;

        public ProductController(IProductRepository productRepository, IFavoriteRepository favoriteRepository)
        {
            _productRepository = productRepository;
            _favoriteRepository = favoriteRepository;
        }

        // GET: api/product
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = await _productRepository.GetAllAsync();
            return Ok(response);
        }

        // GET: api/product/newest?count=4
        [HttpGet("newest")]
        public async Task<IActionResult> GetNewest([FromQuery] int count = 4)
        {
            var response = await _productRepository.GetNewestProductsAsync(count);
            return Ok(response);
        }

        // GET: api/product/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var response = await _productRepository.GetByIdAsync(id);
            if (!response.Success) return NotFound(response);
            return Ok(response);
        }

        // GET: api/product/5/with-favorite-info?userId=1
        [HttpGet("{id}/with-favorite-info")]
        public async Task<IActionResult> GetByIdWithFavoriteInfo(int id, [FromQuery] int? userId = null)
        {
            var productResponse = await _productRepository.GetByIdAsync(id);
            if (!productResponse.Success) return NotFound(productResponse);

            // Lấy số lượng yêu thích
            var favoriteCountResponse = await _favoriteRepository.GetFavoriteCountByProductIdAsync(id);
            int favoriteCount = favoriteCountResponse.Success ? favoriteCountResponse.Data : 0;

            // Kiểm tra user có yêu thích không (nếu có userId)
            bool isFavorite = false;
            if (userId.HasValue && userId.Value > 0)
            {
                var checkFavoriteResponse = await _favoriteRepository.CheckIsFavoriteAsync(userId.Value, id);
                isFavorite = checkFavoriteResponse.Success && checkFavoriteResponse.Data;
            }

            var result = new
            {
                Success = productResponse.Success,
                Message = productResponse.Message,
                Data = new
                {
                    Product = productResponse.Data,
                    FavoriteCount = favoriteCount,
                    IsFavorite = isFavorite
                }
            };

            return Ok(result);
        }

        // POST: api/product
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Product model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var response = await _productRepository.AddAsync(model);
            if (!response.Success) return BadRequest(response);

            return CreatedAtAction(nameof(GetById), new { id = response.Data?.ProductId }, response);
        }

        // PUT: api/product/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Product model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var response = await _productRepository.UpdateAsync(id, model);
            if (!response.Success) return NotFound(response);

            return Ok(response);
        }

        // DELETE: api/product/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _productRepository.DeleteAsync(id);
            if (!response.Success) return NotFound(response);

            return Ok(response);
        }

        // GET: api/product/page?pageNow=1&pageSize=10
        [HttpGet("page")]
        public async Task<IActionResult> GetPage(int pageNow = 1, int pageSize = 10)
        {
            var response = await _productRepository.GetPageAsync(pageNow, pageSize);
            return Ok(response);
        }

        // GET: api/product/category/5
        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetByCategory(int categoryId)
        {
            var response = await _productRepository.GetProductsByCategoryAsync(categoryId);
            return Ok(response);
        }

        // GET: api/product/search?term=laptop
        [HttpGet("search")]
        public async Task<IActionResult> Search(string term)
        {
            var response = await _productRepository.SearchProductsAsync(term);
            return Ok(response);
        }
    }
}
