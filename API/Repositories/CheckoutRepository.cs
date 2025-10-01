using API.Data;
using API.Models;
using API.Models.DTOs;
using API.Repositories.RestAPI;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories
{
    public class CheckoutRepository
    {
        private readonly DataContext _context;

        public CheckoutRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<APIRespone<Order>> CheckoutAsync(CheckoutRequestDto dto)
        {

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1️⃣ Lấy giỏ hàng được chọn
                var cartItems = await _context.Carts
                    .Include(c => c.Product)
                    .Where(c => c.UserId == dto.UserId && dto.CartIds.Contains(c.CartId))
                    .ToListAsync();

                if (!cartItems.Any())
                {
                    return new APIRespone<Order>
                    {
                        Success = false,
                        Message = "Không có sản phẩm nào được chọn"
                    };
                }

                // 2️⃣ Tạo Shipping
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

                // 3️⃣ Tạo Order
                var order = new Order
                {
                    UserId = dto.UserId,
                    ShippingId = shipping.ShippingId,
                    OrderDate = DateTime.Now,
                    Status = OrderStatus.Pending,
                    TotalAmount = cartItems.Sum(i => i.Product.Price * i.Quantity)
                };
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                // 4️⃣ Tạo OrderDetails
                foreach (var item in cartItems)
                {
                    var detail = new OrderDetail
                    {
                        OrderId = order.OrderId,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = item.Product.Price,
                        SubTotal = item.Product.Price * item.Quantity
                    };
                    _context.OrderDetails.Add(detail);
                }
                await _context.SaveChangesAsync();

                // 5️⃣ Tạo Payment
                var payment = new Payment
                {
                    OrderId = order.OrderId,
                    PaymentMethod = dto.PaymentMethod,
                    PaymentStatus = PaymentStatus.Pending
                };
                _context.Payments.Add(payment);
                await _context.SaveChangesAsync();

                // 6️⃣ Xóa các cart item đã chọn
                _context.Carts.RemoveRange(cartItems);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return new APIRespone<Order>
                {
                    Success = true,
                    Message = "Đặt hàng thành công",
                    Data = order
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new APIRespone<Order>
                {
                    Success = false,
                    Message = $"❌ Lỗi khi checkout: {ex.Message}"
                };
            }
        }

    }
}
