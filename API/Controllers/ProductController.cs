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

        public ProductController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        // GET: api/product
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int? page = null, [FromQuery] int? pageSize = null)
        {
            // Nếu có tham số phân trang, trả về dữ liệu phân trang
            if (page.HasValue && pageSize.HasValue)
            {
                var pagedResponse = await _productRepository.GetPageAsync(page.Value, pageSize.Value);
                return Ok(pagedResponse);
            }
            
            // Nếu không có tham số phân trang, trả về tất cả như cũ
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

        // GET: api/product/admin/page?pageNow=1&pageSize=10 - For AdminWeb
        [HttpGet("admin/page")]
        public async Task<IActionResult> GetPageForAdmin(int pageNow = 1, int pageSize = 10)
        {
            var response = await _productRepository.GetPageAsync(pageNow, pageSize);
            
            if (!response.Success)
                return BadRequest(response);

            // Convert Product to ProductViewModel for AdminWeb
            var productViewModels = response.Data.Data.Select(p => new
            {
                ProductId = p.ProductId,
                CategoryId = p.CategoryId,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                StockQuantity = p.StockQuantity,
                ImageUrl = p.ImageUrl,
                CreatedAt = p.CreatedAt,
                Category = p.Category != null ? new
                {
                    CategoryId = p.Category.CategoryId,
                    Name = p.Category.Name
                } : null
            }).ToList();

            var adminPagedResponse = new
            {
                Data = productViewModels,
                PageNow = response.Data.PageNow,
                PageSize = response.Data.PageSize,
                TotalPage = response.Data.TotalPage,
                TotalCount = response.Data.TotalCount
            };

            return Ok(new
            {
                Success = response.Success,
                Data = adminPagedResponse,
                Message = response.Message
            });
        }
    }
}
