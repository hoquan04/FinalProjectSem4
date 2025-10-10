using API.Models;
using API.Repositories.IRepositories;
using API.Repositories.RestAPI;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShipperController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IOrderRepository _orderRepository;

        public ShipperController(IUserRepository userRepository, IOrderRepository orderRepository)
        {
            _userRepository = userRepository;
            _orderRepository = orderRepository;
        }

        /// <summary>
        /// 🚀 Gửi yêu cầu đăng ký làm Shipper (kèm ảnh CCCD mặt trước & sau)
        /// </summary>
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("request-shipper")]
        public async Task<IActionResult> RequestShipper([FromForm] int userId, [FromForm] IFormFile? CccdFront, [FromForm] IFormFile? CccdBack)
        {
            if (userId <= 0 || CccdFront == null || CccdBack == null)
                return BadRequest(new { Success = false, Message = "Thiếu thông tin hoặc ảnh CCCD" });

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "cccd");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            // 🧩 Lưu mặt trước
            var frontFileName = $"{Guid.NewGuid()}_{CccdFront.FileName}";
            var frontPath = Path.Combine(uploadsFolder, frontFileName);
            using (var stream = new FileStream(frontPath, FileMode.Create))
            {
                await CccdFront.CopyToAsync(stream);
            }

            // 🧩 Lưu mặt sau
            var backFileName = $"{Guid.NewGuid()}_{CccdBack.FileName}";
            var backPath = Path.Combine(uploadsFolder, backFileName);
            using (var stream = new FileStream(backPath, FileMode.Create))
            {
                await CccdBack.CopyToAsync(stream);
            }

            // 🧩 URL trả về
            var frontUrl = $"/uploads/cccd/{frontFileName}";
            var backUrl = $"/uploads/cccd/{backFileName}";

            // Gọi repo để lưu DB
            var result = await _userRepository.RequestShipperAsync(userId, frontUrl, backUrl);
            return Ok(result);
        }


        /// <summary>
        /// ✅ (Admin) Phê duyệt hoặc từ chối yêu cầu đăng ký Shipper
        /// </summary>
        [HttpPut("approve")]
        public async Task<IActionResult> ApproveShipperRequest([FromQuery] int userId, [FromQuery] bool isApproved)
        {
            var result = await _userRepository.ApproveShipperRequestAsync(userId, isApproved);
            return Ok(result);
        }

        /// <summary>
        /// 🕓 Lấy danh sách người dùng có yêu cầu Shipper đang chờ duyệt
        /// </summary>
        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingRequests()
        {
            var users = await _userRepository.GetAllUsersAsync();

            if (users.Data == null || !users.Data.Any())
            {
                return Ok(new APIRespone<List<User>>
                {
                    Success = true,
                    Message = "Không có yêu cầu nào chờ duyệt",
                    Data = new List<User>()
                });
            }

            var pending = users.Data
      .Where(u => u.IsShipperRequestPending == true &&
                  (u.Role == UserRole.Customer || u.Role == UserRole.Admin))
      .ToList();


            return Ok(new APIRespone<List<User>>
            {
                Success = true,
                Message = "Danh sách yêu cầu Shipper đang chờ duyệt",
                Data = pending
            });
        }

        /// <summary>
        /// 📦 Lấy danh sách đơn hàng có thể nhận giao
        /// </summary>
        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableOrders()
        {
            var result = await _orderRepository.GetAvailableOrdersForShipperAsync();
            return Ok(result);
        }

        /// <summary>
        /// 🚚 Shipper nhận giao một đơn hàng
        /// </summary>
        [HttpPost("assign")]
        public async Task<IActionResult> AssignOrder([FromQuery] int orderId, [FromQuery] int shipperId)
        {
            var result = await _orderRepository.AssignOrderToShipperAsync(orderId, shipperId);
            return Ok(result);
        }

        /// <summary>
        /// 📋 Danh sách đơn của shipper
        /// </summary>
        [HttpGet("my-orders/{shipperId}")]
        public async Task<IActionResult> GetMyOrders(int shipperId)
        {
            var result = await _orderRepository.GetOrdersOfShipperAsync(shipperId);
            return Ok(result);
        }

        /// <summary>
        /// ✅ Shipper hoàn tất đơn hàng
        /// </summary>
        [HttpPut("complete")]
        public async Task<IActionResult> CompleteOrder([FromQuery] int orderId, [FromQuery] int shipperId)
        {
            var result = await _orderRepository.CompleteOrderByShipperAsync(orderId, shipperId);
            return Ok(result);
        }
    }

    /// <summary>
    /// DTO cho yêu cầu đăng ký Shipper
    /// </summary>
    public class RequestShipperDto
    {
        public int UserId { get; set; }
        public string CccdFrontUrl { get; set; } = string.Empty;
        public string CccdBackUrl { get; set; } = string.Empty;
    }
}
