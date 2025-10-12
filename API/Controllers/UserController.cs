using API.Models;
using API.Repositories.IRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    public UserController(IUserRepository userRepository) { _userRepository = userRepository; }

    // Tùy bạn: chỉ Admin mới xem danh sách người dùng?
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
    {
        var response = await _userRepository.GetAllUsersAsync();
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var response = await _userRepository.GetUserByIdAsync(id);
        if (!response.Success || response.Data == null) return NotFound(response);
        return Ok(response);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")] // ✅ chỉ Admin được tạo và chọn Role
    public async Task<IActionResult> Create([FromBody] User model)
    {
        // ✅ Không validate PasswordHash (đã BindNever + nullable nhưng thêm cho chắc)
        ModelState.Remove(nameof(API.Models.User.PasswordHash));

        if (string.IsNullOrWhiteSpace(model.Password))
            return BadRequest(new { message = "Password là bắt buộc" });

        if (!ModelState.IsValid) return BadRequest(ModelState);

        var response = await _userRepository.CreateUserAsync(model);
        if (!response.Success) return BadRequest(response);

        return CreatedAtAction(nameof(GetById), new { id = response.Data?.UserId }, response);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] User model)
    {
        ModelState.Remove(nameof(API.Models.User.PasswordHash));
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var response = await _userRepository.UpdateUserAsync(id, model);
        if (!response.Success) return NotFound(response);
        return Ok(response);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var response = await _userRepository.DeleteUserAsync(id);
        if (!response.Success) return NotFound(response);
        return Ok(response);
    }

    [HttpGet("page")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetPage(int pageNow = 1, int pageSize = 10)
    {
        var response = await _userRepository.GetPageAsync(pageNow, pageSize);
        return Ok(response);
    }

    [HttpGet("search")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Search([FromQuery] string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword))
            return BadRequest(new { Success = false, Message = "Keyword không được để trống" });

        var response = await _userRepository.SearchUsersAsync(keyword);
        return Ok(response);
    }
}
