
using API.Data;
using API.Models;
using API.Repositories.IRepositories;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController] // Đánh dấu là API Controller
    [Route("api/[controller]")] // => route mặc định: api/user
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        //private readonly DataContext _context;

        // Inject repository và DataContext
        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
            //_context = context;
        }

        // GET: api/user
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = await _userRepository.GetAllUsersAsync();
            return Ok(response);
        }

        // GET: api/user/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var response = await _userRepository.GetUserByIdAsync(id);

            if (!response.Success || response.Data == null)
                return NotFound(response);

            return Ok(response);
        }

        // POST: api/user
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] User model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var response = await _userRepository.CreateUserAsync(model);
            if (!response.Success) return BadRequest(response);

            return CreatedAtAction(nameof(GetById), new { id = response.Data?.UserId }, response);
        }

        // PUT: api/user/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] User model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var response = await _userRepository.UpdateUserAsync(id, model);
            if (!response.Success) return NotFound(response);

            return Ok(response);
        }

        // DELETE: api/user/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _userRepository.DeleteUserAsync(id);
            if (!response.Success) return NotFound(response);

            return Ok(response);
        }

        // GET: api/user/page?pageNow=1&pageSize=10
        [HttpGet("page")]
        public async Task<IActionResult> GetPage(int pageNow = 1, int pageSize = 10)
        {
            var response = await _userRepository.GetPageAsync(pageNow, pageSize);
            return Ok(response);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return BadRequest(new { Success = false, Message = "Keyword không được để trống" });

            var response = await _userRepository.SearchUsersAsync(keyword);

            return Ok(response);
        }
    }
}
