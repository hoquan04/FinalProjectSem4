//using API.Data;
//using API.Models;
//using API.Repositories.IRepositories;
//using API.Repositories.Services;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;

//namespace API.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class VnPayController : ControllerBase
//    {
//        private readonly IVnPayService _vnPayService;
//        private readonly DataContext _context;
//        private readonly IEmailSender _emailSender;
//        private readonly INotificationRepository _notificationRepo;

//        public VnPayController(
//            IVnPayService vnPayService,
//            DataContext context,
//            IEmailSender emailSender,
//            INotificationRepository notificationRepo)
//        {
//            _vnPayService = vnPayService;
//            _context = context;
//            _emailSender = emailSender;
//            _notificationRepo = notificationRepo;
//        }

//[HttpGet("callback")]
//public async Task<IActionResult> Callback(
//    [FromQuery] int userId,
//    [FromQuery] double total,
//    [FromQuery] string recipientName,
//    [FromQuery] string phone,
//    [FromQuery] string email,
//    [FromQuery] string address)
//{
//    var response = _vnPayService.PaymentExecute(Request.Query);

//    // ❌ Nếu người dùng hủy thanh toán hoặc lỗi
//    if (response.VnPayResponseCode != "00")
//        return Ok(new { success = false, message = "Thanh toán thất bại hoặc bị hủy." });

//    // ✅ Nếu thanh toán thành công
//    using var transaction = await _context.Database.BeginTransactionAsync();
//    try
//    {
//        // 1️⃣ Lấy danh sách sản phẩm trong giỏ của user
//        var cartItems = await _context.Carts
//            .Include(c => c.Product)
//            .Where(c => c.UserId == userId)
//            .ToListAsync();

//        if (!cartItems.Any())
//            return BadRequest(new { success = false, message = "Giỏ hàng trống." });

//        // 2️⃣ Tạo Shipping
//        var shipping = new Shipping
//        {
//            RecipientName = recipientName,
//            PhoneNumber = phone,
//            Email = email,
//            Address = address
//        };
//        _context.Shippings.Add(shipping);
//        await _context.SaveChangesAsync();

//        // 3️⃣ Tạo Order
//        var order = new Order
//        {
//            UserId = userId,
//            ShippingId = shipping.ShippingId,
//            Status = OrderStatus.Pending, // ✅ vẫn ở trạng thái "đang xử lý"
//            TotalAmount = (decimal)total
//        };
//        _context.Orders.Add(order);
//        await _context.SaveChangesAsync();

//        // 4️⃣ Thêm chi tiết sản phẩm
//        foreach (var item in cartItems)
//        {
//            _context.OrderDetails.Add(new OrderDetail
//            {
//                OrderId = order.OrderId,
//                ProductId = item.ProductId,
//                Quantity = item.Quantity,
//                UnitPrice = item.Product.Price,
//                SubTotal = item.Product.Price * item.Quantity
//            });
//        }
//        await _context.SaveChangesAsync();

//        // 5️⃣ Thêm thông tin thanh toán
//        _context.Payments.Add(new Payment
//        {
//            OrderId = order.OrderId,
//            PaymentMethod = PaymentMethod.VNPay,
//            PaymentStatus = PaymentStatus.Paid,
//            PaidAt = DateTime.Now
//        });
//        await _context.SaveChangesAsync();

//        // 6️⃣ Xóa giỏ hàng
//        _context.Carts.RemoveRange(cartItems);
//        await _context.SaveChangesAsync();

//        await transaction.CommitAsync();

//        // 🔔 Gửi thông báo
//        await _notificationRepo.AddAsync(new Notification
//        {
//            UserId = order.UserId,
//            OrderId = order.OrderId,
//            Title = "Thanh toán VNPay thành công",
//            Message = $"Đơn hàng #{order.OrderId} đã được thanh toán thành công.",
//            Type = NotificationType.Payment,
//            CreatedAt = DateTime.UtcNow
//        });

//        // 📧 Gửi email
//        if (!string.IsNullOrEmpty(email))
//        {
//            await _emailSender.SendEmailAsync(
//                email,
//                $"Thanh toán đơn hàng #{order.OrderId} thành công",
//                $"<p>Bạn đã thanh toán thành công đơn hàng #{order.OrderId} qua VNPay.</p>"
//            );
//        }

//        // ✅ Trả kết quả cho Flutter
//        return Ok(new { success = true, message = "Thanh toán thành công", orderId = order.OrderId });
//    }
//    catch (Exception ex)
//    {
//        await transaction.RollbackAsync();
//        return BadRequest(new { success = false, message = ex.Message });
//    }
//}
//        }
//}

using API.Data;
using API.Models;
using API.Repositories.IRepositories;
using API.Repositories.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VnPayController : ControllerBase
    {
        private readonly IVnPayService _vnPayService;
        private readonly DataContext _context;
        private readonly IEmailSender _emailSender;
        private readonly INotificationRepository _notificationRepo;

        public VnPayController(
            IVnPayService vnPayService,
            DataContext context,
            IEmailSender emailSender,
            INotificationRepository notificationRepo)
        {
            _vnPayService = vnPayService;
            _context = context;
            _emailSender = emailSender;
            _notificationRepo = notificationRepo;
        }

        [HttpGet("callback")]
        public async Task<IActionResult> Callback(
            [FromQuery] int userId,
            [FromQuery] string cartIds,
            [FromQuery] double total,
            [FromQuery] string recipientName,
            [FromQuery] string phone,
            [FromQuery] string email,
            [FromQuery] string address)
        {
            var response = _vnPayService.PaymentExecute(Request.Query);

            // ❌ Nếu thanh toán thất bại
            if (response == null || response.VnPayResponseCode != "00")
            {
                return Ok(new
                {
                    success = false,
                    message = "Thanh toán thất bại hoặc bị hủy."
                });
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 🔹 Lấy danh sách cart
                if (string.IsNullOrEmpty(cartIds))
                    return BadRequest(new { success = false, message = "Thiếu danh sách giỏ hàng cần thanh toán." });

                var selectedIds = cartIds.Split(',').Select(int.Parse).ToList();

                var cartItems = await _context.Carts
                    .Include(c => c.Product)
                    .Where(c => c.UserId == userId && selectedIds.Contains(c.CartId))
                    .ToListAsync();

                if (!cartItems.Any())
                    return BadRequest(new { success = false, message = "Giỏ hàng trống." });

                // 1️⃣ Shipping
                var shipping = new Shipping
                {
                    RecipientName = recipientName,
                    PhoneNumber = phone,
                    Email = email,
                    Address = address
                };
                _context.Shippings.Add(shipping);
                await _context.SaveChangesAsync();

                // 2️⃣ Order
                var order = new Order
                {
                    UserId = userId,
                    ShippingId = shipping.ShippingId,
                    Status = OrderStatus.Pending,
                    TotalAmount = (decimal)total
                };
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                // 3️⃣ Order Details
                foreach (var item in cartItems)
                {
                    _context.OrderDetails.Add(new OrderDetail
                    {
                        OrderId = order.OrderId,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = item.Product.Price,
                        SubTotal = item.Product.Price * item.Quantity
                    });
                }
                await _context.SaveChangesAsync();

                // 4️⃣ Payment
                _context.Payments.Add(new Payment
                {
                    OrderId = order.OrderId,
                    PaymentMethod = PaymentMethod.VNPay,
                    PaymentStatus = PaymentStatus.Paid,
                    PaidAt = DateTime.Now
                });
                await _context.SaveChangesAsync();

                // 5️⃣ Xóa giỏ hàng
                _context.Carts.RemoveRange(cartItems);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                // 6️⃣ Thông báo
                await _notificationRepo.AddAsync(new Notification
                {
                    UserId = order.UserId,
                    OrderId = order.OrderId,
                    Title = "Đơn hàng mới",
                    Message = $"Đơn hàng #{order.OrderId} đã được tạo thành công và đang chờ xử lý.",
                    Type = NotificationType.Order,
                    CreatedAt = DateTime.UtcNow,
                    IsRead = false
                });

                await _notificationRepo.AddAsync(new Notification
                {
                    UserId = order.UserId,
                    OrderId = order.OrderId,
                    Title = "Thanh toán VNPay thành công",
                    Message = $"Bạn đã thanh toán thành công cho đơn hàng #{order.OrderId} qua VNPay.",
                    Type = NotificationType.Payment,
                    CreatedAt = DateTime.UtcNow,
                    IsRead = false
                });

                // 7️⃣ 📧 Gửi email xác nhận (giống COD)
                if (!string.IsNullOrEmpty(email))
                {
                    await _emailSender.SendEmailAsync(
                        email,
                        $"Xác nhận đơn hàng #{order.OrderId}",
                        $@"
                        <p>Xin chào {recipientName},</p>
                        <p>Bạn đã thanh toán thành công đơn hàng <strong>#{order.OrderId}</strong> qua VNPay.</p>
                        <p>Phương thức thanh toán: <strong>VNPay (Đã thanh toán)</strong>.</p>
                        <p>Chúng tôi sẽ liên hệ và giao hàng trong thời gian sớm nhất.</p>
                        <p>Cảm ơn bạn đã mua sắm tại cửa hàng của chúng tôi ❤️</p>
                        "
                    );
                }

                // ✅ Trả về Flutter
                return Ok(new
                {
                    success = true,
                    message = "Thanh toán VNPay thành công",
                    orderId = order.OrderId
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return BadRequest(new
                {
                    success = false,
                    message = $"Lỗi xử lý callback: {ex.Message}"
                });
            }
        }
    }
}
