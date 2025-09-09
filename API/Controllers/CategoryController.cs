using API.Models;
using API.Repositories.IRepositories;
using API.Repositories.RestAPI;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;

        // Inject repository thông qua constructor
        public CategoryController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        // GET: api/category
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = await _categoryRepository.GetAllAsync();
            return Ok(response);
        }

        // GET: api/category/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var response = await _categoryRepository.GetByIdAsync(id);
            if (!response.Success) return NotFound(response);
            return Ok(response);
        }

        // POST: api/category
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Category model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var response = await _categoryRepository.AddAsync(model);
            if (!response.Success) return BadRequest(response);

            return CreatedAtAction(nameof(GetById), new { id = response.Data?.CategoryId }, response);
        }

        // PUT: api/category/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Category model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var response = await _categoryRepository.UpdateAsync(id, model);
            if (!response.Success) return NotFound(response);

            return Ok(response);
        }

        // DELETE: api/category/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _categoryRepository.DeleteAsync(id);
            if (!response.Success) return NotFound(response);

            return Ok(response);
        }

        // GET: api/category/page?pageNow=1&pageSize=10
        [HttpGet("page")]
        public async Task<IActionResult> GetPage(int pageNow = 1, int pageSize = 10)
        {
            var response = await _categoryRepository.GetPageAsync(pageNow, pageSize);
            return Ok(response);
        }
    }
}
