using API.Data;
using API.Models;
using API.Models.DTOs;
using API.Repositories.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VnPayController : ControllerBase
    {
        private readonly IVnPayService _vnPayService;

        public VnPayController(IVnPayService vnPayService)
        {
            _vnPayService = vnPayService;
        }

        [HttpPost("create")]
        public IActionResult CreatePaymentUrl([FromBody] VnPaymentRequestModel model)
        {
            var url = _vnPayService.CreatePaymentUrl(HttpContext, model);
            return Ok(new { paymentUrl = url });
        }

        [HttpGet("callback")]
        public async Task<IActionResult> PaymentCallback()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);

            // ❌ Nếu chữ ký sai hoặc bị hủy
            if (!response.Success || response.VnPayResponseCode != "00")
                return Redirect("https://flutter-fail-page"); // hoặc gửi lại app

            // ✅ Nếu thanh toán thành công
            int userId = int.Parse(Request.Query["userId"]);
            decimal total = decimal.Parse(Request.Query["amount"]);

            using var scope = HttpContext.RequestServices.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<DataContext>();

            // Lấy giỏ hàng thật của user
            var cartItems = await db.Carts.Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .ToListAsync();

            if (!cartItems.Any())
                return BadRequest("Không tìm thấy sản phẩm trong giỏ hàng");

            // Tạo Shipping
            var shipping = new Shipping
            {
                RecipientName = "Khách VNPay",
                PhoneNumber = "0000000",
                Address = "Địa chỉ VNPay",
                CreatedAt = DateTime.Now
            };
            db.Shippings.Add(shipping);
            await db.SaveChangesAsync();

            // Tạo Order
            var order = new Order
            {
                UserId = userId,
                ShippingId = shipping.ShippingId,
                Status = OrderStatus.Confirmed,
                TotalAmount = total
            };
            db.Orders.Add(order);
            await db.SaveChangesAsync();

            // Tạo OrderDetail
            foreach (var item in cartItems)
            {
                db.OrderDetails.Add(new OrderDetail
                {
                    OrderId = order.OrderId,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.Product.Price,
                    SubTotal = item.Product.Price * item.Quantity
                });
            }

            // Tạo Payment
            db.Payments.Add(new Payment
            {
                OrderId = order.OrderId,
                PaymentMethod = PaymentMethod.VNPay,
                PaymentStatus = PaymentStatus.Paid,
                PaidAt = DateTime.Now
            });

            db.Carts.RemoveRange(cartItems);
            await db.SaveChangesAsync();

            // ✅ Redirect về Flutter (thành công)
            return Redirect("https://flutterapp/callback-success");
        }



    }
}
