using API.Data;
using API.Models;
using API.Models.DTOs;
using API.Repositories.IRepositories;
using API.Repositories.RestAPI;
using API.Repositories.Services;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories
{
    public class CheckoutRepository
    {
        private readonly DataContext _context;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEmailSender _emailSender;
        private readonly IVnPayService _vnPayService;
        private readonly INotificationRepository _notificationRepo;

        public CheckoutRepository(
            DataContext context,
            IConfiguration config,
            IHttpContextAccessor httpContextAccessor,
            IEmailSender emailSender,
            IVnPayService vnPayService,
            INotificationRepository notificationRepo) // 👈 thêm vào đây
        {
            _context = context;
            _config = config;
            _httpContextAccessor = httpContextAccessor;
            _emailSender = emailSender;
            _vnPayService = vnPayService;
            _notificationRepo = notificationRepo; // 👈 gán vào biến
        }


        public async Task<APIRespone<object>> CheckoutAsync(CheckoutRequestDto dto)
        {
            // 🛒 Lấy danh sách sản phẩm trong giỏ
            var cartItems = await _context.Carts
                .Include(c => c.Product)
                .Where(c => c.UserId == dto.UserId && dto.CartIds.Contains(c.CartId))
                .ToListAsync();

            if (!cartItems.Any())
                return new APIRespone<object> { Success = false, Message = "Không có sản phẩm nào được chọn" };

            // 🔢 Tính tổng tiền
            var totalAmount = cartItems.Sum(i => i.Product.Price * i.Quantity);

            // 🔹 Nếu là COD → Lưu luôn
            if (dto.PaymentMethod == PaymentMethod.CashOnDelivery)
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    var shipping = new Shipping
                    {
                        RecipientName = dto.RecipientName,
                        PhoneNumber = dto.PhoneNumber,
                        Email = dto.Email,
                        Address = dto.Address,
                        City = dto.City,
                        PostalCode = dto.PostalCode
                    };
                    _context.Shippings.Add(shipping);
                    await _context.SaveChangesAsync();

                    var order = new Order
                    {
                        UserId = dto.UserId,
                        ShippingId = shipping.ShippingId,
                        Status = OrderStatus.Pending,
                        TotalAmount = totalAmount
                    };
                    _context.Orders.Add(order);
                    await _context.SaveChangesAsync();

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

                    _context.Payments.Add(new Payment
                    {
                        OrderId = order.OrderId,
                        PaymentMethod = PaymentMethod.CashOnDelivery,
                        PaymentStatus = PaymentStatus.Pending
                    });
                    await _context.SaveChangesAsync();

                    _context.Carts.RemoveRange(cartItems);
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();

                    // 🔔 Tạo thông báo cho COD
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
                        Title = "Thanh toán khi nhận hàng",
                        Message = $"Bạn đã chọn phương thức thanh toán tiền mặt khi nhận hàng cho đơn #{order.OrderId}.",
                        Type = NotificationType.Payment,
                        CreatedAt = DateTime.UtcNow,
                        IsRead = false
                    });

                    // 📧 Gửi email xác nhận đơn hàng COD
                    if (!string.IsNullOrEmpty(dto.Email))
                    {
                        await _emailSender.SendEmailAsync(
                            dto.Email,
                            $"Xác nhận đơn hàng #{order.OrderId}",
                            $@"
            <p>Xin chào {dto.RecipientName},</p>
            <p>Bạn đã đặt đơn hàng <strong>#{order.OrderId}</strong> thành công.</p>
            <p>Phương thức thanh toán: <strong>Tiền mặt khi nhận hàng (COD)</strong>.</p>
            <p>Chúng tôi sẽ liên hệ và giao hàng trong thời gian sớm nhất.</p>
            <p>Cảm ơn bạn đã mua sắm tại cửa hàng của chúng tôi ❤️</p>
        "
                        );
                    }
                    return new APIRespone<object>
                    {
                        Success = true,
                        Message = "Đặt hàng thành công (COD)",
                        Data = order
                    };

                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new APIRespone<object> { Success = false, Message = ex.Message };
                }
            }

            // 🔹 Nếu là VNPay → KHÔNG LƯU DB, chỉ tạo URL thanh toán
            if (dto.PaymentMethod == PaymentMethod.VNPay)
            {
                var total = (double)totalAmount;
                var vnRequest = new VnPaymentRequestModel
                {
                    OrderId = (int)(DateTime.Now.Ticks % int.MaxValue),
                    FullName = dto.RecipientName,
                    Amount = total,
                    Description = $"Thanh toán đơn hàng cho user {dto.UserId}",
                    CreatedDate = DateTime.Now
                };

                // ✅ Gửi tất cả dữ liệu cần thiết cho callback
                // 🔹 Gửi thêm danh sách CartIds qua URL để callback chỉ xử lý sản phẩm đã chọn
                var cartIds = string.Join(",", dto.CartIds); // ví dụ: "5,7,10"
                var queryString = $"userId={dto.UserId}&cartIds={cartIds}&total={total}&recipientName={dto.RecipientName}&phone={dto.PhoneNumber}&email={dto.Email}&address={dto.Address}";

                var returnUrl = $"{_config["Vnpay:PaymentBackReturnUrl"]}?{queryString}";
                var paymentUrl = _vnPayService.CreatePaymentUrl(_httpContextAccessor.HttpContext!, vnRequest, returnUrl);

                return new APIRespone<object>
                {
                    Success = true,
                    Message = "Chuyển hướng đến VNPay để thanh toán",
                    Data = new { PaymentUrl = paymentUrl }
                };
            }

            return new APIRespone<object> { Success = false, Message = "Phương thức thanh toán không hợp lệ" };
        }

    }
}
