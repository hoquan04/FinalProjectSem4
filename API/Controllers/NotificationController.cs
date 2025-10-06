using API.Models;
using API.Repositories.IRepositories;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationRepository _notificationRepository;

        public NotificationController(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = await _notificationRepository.GetAllAsync();
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var response = await _notificationRepository.GetByIdAsync(id);
            if (!response.Success)
                return NotFound(response);
            return Ok(response);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUserId(int userId)
        {
            var response = await _notificationRepository.GetByUserIdAsync(userId);
            return Ok(response);
        }

        [HttpGet("user/{userId}/unread-count")]
        public async Task<IActionResult> GetUnreadCount(int userId)
        {
            var response = await _notificationRepository.GetUnreadCountAsync(userId);
            return Ok(response);
        }

        [HttpGet("page")]
        public async Task<IActionResult> GetPage([FromQuery] int pageNow = 1, [FromQuery] int pageSize = 10)
        {
            var response = await _notificationRepository.GetPageAsync(pageNow, pageSize);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Notification notification)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _notificationRepository.AddAsync(notification);
            return CreatedAtAction(nameof(GetById), new { id = notification.NotificationId }, response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Notification notification)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _notificationRepository.UpdateAsync(id, notification);
            if (!response.Success)
                return NotFound(response);
            return Ok(response);
        }

        [HttpPut("{id}/mark-read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var response = await _notificationRepository.MarkAsReadAsync(id);
            if (!response.Success)
                return NotFound(response);
            return Ok(response);
        }

        [HttpPut("user/{userId}/mark-all-read")]
        public async Task<IActionResult> MarkAllAsRead(int userId)
        {
            var response = await _notificationRepository.MarkAllAsReadAsync(userId);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _notificationRepository.DeleteAsync(id);
            if (!response.Success)
                return NotFound(response);
            return Ok(response);
        }

        [HttpDelete("user/{userId}/clear-all")]
        public async Task<IActionResult> DeleteAllByUserId(int userId)
        {
            var response = await _notificationRepository.DeleteAllByUserIdAsync(userId);
            return Ok(response);
        }
    }
}
