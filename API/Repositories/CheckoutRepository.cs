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
            private readonly IConfiguration _config;
            private readonly IHttpContextAccessor _httpContextAccessor;
            public CheckoutRepository(DataContext context, IConfiguration config, IHttpContextAccessor httpContextAccessor)
            {
                _context = context;
                _config = config;
                _httpContextAccessor = httpContextAccessor;
            }
            public async Task<APIRespone<object>> CheckoutAsync(CheckoutRequestDto dto)
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    // 1️⃣ Lấy cart
                    var cartItems = await _context.Carts
                        .Include(c => c.Product)
                        .Where(c => c.UserId == dto.UserId && dto.CartIds.Contains(c.CartId))
                        .ToListAsync();

                    if (!cartItems.Any())
                    {
                        return new APIRespone<object>
                        {
                            Success = false,
                            Message = "Không có sản phẩm nào được chọn"
                        };
                    }

                    // 2️⃣ Shipping
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

                    // 3️⃣ Order
                    var order = new Order
                    {
                        UserId = dto.UserId,
                        ShippingId = shipping.ShippingId,
                        Status = OrderStatus.Pending,
                        TotalAmount = cartItems.Sum(i => i.Product.Price * i.Quantity)
                    };
                    _context.Orders.Add(order);
                    await _context.SaveChangesAsync();

                    // 4️⃣ OrderDetails
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

                    // 5️⃣ Payment (pending)
                    // 5️⃣ Payment
                    var payment = new Payment
                    {
                        OrderId = order.OrderId,
                        PaymentMethod = dto.PaymentMethod,
                        PaymentStatus = dto.PaymentMethod == PaymentMethod.VNPay
                            ? PaymentStatus.Paid
                            : PaymentStatus.Pending,
                        PaidAt = dto.PaymentMethod == PaymentMethod.VNPay
                            ? DateTime.Now
                            : null
                    };
                    _context.Payments.Add(payment);
                    await _context.SaveChangesAsync();

                

                    // 6️⃣ Clear cart
                    _context.Carts.RemoveRange(cartItems);
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();

                    // 🔄 Trả kết quả theo PaymentMethod
                    if (dto.PaymentMethod == PaymentMethod.CashOnDelivery)
                    {
                        return new APIRespone<object>
                        {
                            Success = true,
                            Message = "Đặt hàng thành công (COD)",
                            Data = order
                        };
                    }
                else if (dto.PaymentMethod == PaymentMethod.VNPay)
                {
                    var total = cartItems.Sum(i => i.Product.Price * i.Quantity);

                    // 🔹 Không lưu order, payment, shipping gì cả ở đây
                    var vnRequest = new VnPaymentRequestModel
                    {
                        OrderId = (int)(DateTime.Now.Ticks % int.MaxValue), // mã tạm thời
                        FullName = dto.RecipientName,
                        Amount = (double)total,
                        Description = $"Thanh toán đơn hàng tạm cho user {dto.UserId}",
                        CreatedDate = DateTime.Now
                    };

                    // thêm userId & total để callback nhận
                    var returnUrl = $"{_config["Vnpay:PaymentBackReturnUrl"]}?userId={dto.UserId}&amount={total}";
                    var vnpay = new VnPayService(_config);
                    var url = vnpay.CreatePaymentUrl(_httpContextAccessor.HttpContext, vnRequest, returnUrl);

                    // ⚠️ KHÔNG lưu database
                    return new APIRespone<object>
                    {
                        Success = true,
                        Message = "Chuyển hướng đến VNPay để thanh toán",
                        Data = new { PaymentUrl = url }
                    };
                }



                return new APIRespone<object>
                    {
                        Success = false,
                        Message = "Phương thức thanh toán không được hỗ trợ"
                    };
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new APIRespone<object>
                    {
                        Success = false,
                        Message = $"❌ Lỗi khi checkout: {ex.Message}"
                    };
                }
            }


        }
    }
