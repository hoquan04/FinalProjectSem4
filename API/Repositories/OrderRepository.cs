using API.Data;
using API.Models;
using API.Models.DTOs;
using API.Repositories.IRepositories;
using API.Repositories.RestAPI;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly DataContext _context;
        private readonly IEmailSender _emailSender;
        private readonly INotificationRepository _notificationRepo;

        public OrderRepository(DataContext context, IEmailSender emailSender, INotificationRepository notificationRepo)
        {
            _context = context;
            _emailSender = emailSender;
            _notificationRepo = notificationRepo;
        }


        public async Task<APIRespone<Order>> AddAsync(Order entity)
        {
            _context.Orders.Add(entity);
            await _context.SaveChangesAsync();

            // 🆕 Gửi thông báo xác nhận đơn hàng mới
            await _notificationRepo.AddAsync(new Notification
            {
                UserId = entity.UserId,
                OrderId = entity.OrderId,
                Title = "Đặt hàng thành công",
                Message = $"Cảm ơn bạn đã đặt hàng #{entity.OrderId}. Chúng tôi sẽ xử lý đơn hàng sớm nhất!",
                Type = NotificationType.Order,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            });

            return new APIRespone<Order>
            {
                Success = true,
                Message = "Thêm đơn hàng thành công",
                Data = entity
            };
        }

        public async Task<APIRespone<bool>> DeleteAsync(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Notifications)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
                return new APIRespone<bool> { Success = false, Message = "Không tìm thấy đơn hàng", Data = false };

            // ✅ Xóa notification liên quan
            if (order.Notifications != null)
                _context.Notifications.RemoveRange(order.Notifications);

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return new APIRespone<bool> { Success = true, Message = "Xóa đơn hàng thành công", Data = true };
        }


        public async Task<APIRespone<IEnumerable<Order>>> GetAllAsync()
        {
            var orders = await _context.Orders
                .Include(o => o.Users)
                .Include(o => o.Shipping)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .Include(o => o.Payments)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return new APIRespone<IEnumerable<Order>>
            {
                Success = true,
                Message = "Lấy danh sách tất cả đơn hàng thành công",
                Data = orders
            };
        }

        public async Task<APIRespone<Order>> GetByIdAsync(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Users)
                .Include(o => o.Shipping)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .Include(o => o.Payments)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
            {
                return new APIRespone<Order>
                {
                    Success = false,
                    Message = "Không tìm thấy đơn hàng",
                    Data = null
                };
            }

            return new APIRespone<Order>
            {
                Success = true,
                Message = "Lấy đơn hàng thành công",
                Data = order
            };
        }

        public async Task<APIRespone<Order>> UpdateAsync(int id, Order entity)
        {
            var order = await _context.Orders
                .Include(o => o.Shipping)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
            {
                return new APIRespone<Order>
                {
                    Success = false,
                    Message = "Không tìm thấy đơn hàng",
                    Data = null
                };
            }

            _context.Entry(order).CurrentValues.SetValues(entity);
            await _context.SaveChangesAsync();
            // 🧠 Gửi thông báo cho người dùng khi trạng thái đơn hàng thay đổi
            await _notificationRepo.AddAsync(new Notification
            {
                UserId = order.UserId,
                OrderId = order.OrderId,
                Title = "Cập nhật trạng thái đơn hàng",
                Message = $"Đơn hàng #{order.OrderId} đã được cập nhật sang trạng thái: {order.Status}.",
                Type = NotificationType.Order,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            });

            try
            {
                var email = order.Shipping?.Email;
                if (!string.IsNullOrEmpty(email))
                {
                    string subject = $"Cập nhật trạng thái đơn hàng #{order.OrderId}";
                    string message = $@"
                        <h3>Xin chào {order.Shipping?.RecipientName},</h3>
                        <p>Đơn hàng <strong>#{order.OrderId}</strong> đã được cập nhật.</p>
                        <p><b>Trạng thái mới:</b> {order.Status}</p>
                        <p><b>Ngày đặt:</b> {order.OrderDate:dd/MM/yyyy}</p>
                        <p><b>Tổng tiền:</b> {order.TotalAmount:N0} đ</p>
                        <br/>
                        <p>Cảm ơn bạn đã mua sắm tại cửa hàng!</p>";

                    await _emailSender.SendEmailAsync(email, subject, message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi gửi email: {ex.Message}");
            }

            return new APIRespone<Order>
            {
                Success = true,
                Message = "Cập nhật đơn hàng thành công và đã gửi thông báo email",
                Data = entity
            };
        }

        public async Task<APIRespone<PagedResponse<Order>>> GetPageAsync(int pageNow, int pageSize)
        {
            var query = _context.Orders
                .Include(o => o.Users)
                .Include(o => o.Shipping)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .Include(o => o.Payments)
                .OrderByDescending(o => o.OrderDate)
                .AsQueryable();

            var totalCount = await query.CountAsync();
            var totalPage = (int)Math.Ceiling(totalCount / (double)pageSize);

            var data = await query
                .Skip((pageNow - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var response = new PagedResponse<Order>
            {
                Data = data,
                PageNow = pageNow,
                PageSize = pageSize,
                TotalPage = totalPage,
                TotalCount = totalCount
            };

            return new APIRespone<PagedResponse<Order>>
            {
                Success = true,
                Message = "Lấy danh sách đơn hàng phân trang thành công",
                Data = response
            };
        }

        public async Task<APIRespone<PagedResponse<Order>>> Search(int pageNow, int pageSize, SearchOrder search)
        {
            var query = _context.Orders
                .Include(o => o.Users)
                .Include(o => o.Shipping)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .Include(o => o.Payments)
                .AsQueryable();

            // 🔎 Tìm theo từ khóa
            if (!string.IsNullOrEmpty(search.Keyword))
            {
                var keyword = search.Keyword.ToLower();

                if (int.TryParse(search.Keyword, out int orderId))
                {
                    // Nếu nhập số thì tìm theo Mã đơn
                    query = query.Where(o => o.OrderId == orderId);
                }
                else if (Enum.TryParse<OrderStatus>(search.Keyword, true, out var status))
                {
                    // Nếu nhập đúng tên trạng thái thì tìm theo Trạng thái
                    query = query.Where(o => o.Status == status);
                }
                else
                {
                    query = query.Where(o =>
                         (o.Shipping != null && (
                             (o.Shipping.RecipientName ?? "").ToLower().Contains(keyword) ||
                             (o.Shipping.Email ?? "").ToLower().Contains(keyword) ||
                             (o.Shipping.Address ?? "").ToLower().Contains(keyword)
                         ))
                     );

                }
            }

            // 📅 Lọc theo ngày
            // 📅 Lọc theo ngày - SỬA LẠI
            if (search.FromDate.HasValue)
            {
                var from = search.FromDate.Value.Date;
                query = query.Where(o => o.OrderDate >= from);
            }

            if (search.ToDate.HasValue)
            {
                var to = search.ToDate.Value.Date.AddDays(1).AddSeconds(-1);
                query = query.Where(o => o.OrderDate <= to);
            }

            // 💰 Lọc theo tổng tiền
            if (search.MinAmount.HasValue)
            {
                query = query.Where(o => o.TotalAmount >= search.MinAmount.Value);
            }
            if (search.MaxAmount.HasValue)
            {
                query = query.Where(o => o.TotalAmount <= search.MaxAmount.Value);
            }

            // 📌 Phân trang
            var totalCount = await query.CountAsync();
            var totalPage = (int)Math.Ceiling(totalCount / (double)pageSize);

            var data = await query
                .OrderByDescending(o => o.OrderDate)
                .Skip((pageNow - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var response = new PagedResponse<Order>
            {
                Data = data,
                PageNow = pageNow,
                PageSize = pageSize,
                TotalPage = totalPage,
                TotalCount = totalCount
            };

            return new APIRespone<PagedResponse<Order>>
            {
                Success = true,
                Message = "Tìm kiếm đơn hàng thành công",
                Data = response
            };
        }
        public async Task<APIRespone<IEnumerable<Order>>> GetByUserIdAsync(int userId)
        {
            var orders = await _context.Orders
                .Include(o => o.Shipping)
                .Include(o => o.OrderDetails).ThenInclude(od => od.Product)
                .Include(o => o.Payments)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return new APIRespone<IEnumerable<Order>>
            {
                Success = true,
                Message = "Lấy lịch sử đơn hàng theo UserId thành công",
                Data = orders
            };
        }




        public async Task<APIRespone<IEnumerable<Order>>> GetOrdersForShipperAsync()
        {
            var orders = await _context.Orders
                .Include(o => o.Shipping)
                .Include(o => o.Payments)
                .Where(o => o.Status == OrderStatus.Confirmed || o.Status == OrderStatus.Shipping)
                .ToListAsync();

            return new APIRespone<IEnumerable<Order>>
            {
                Success = true,
                Data = orders,
                Message = "Danh sách đơn hàng cần giao"
            };
        }


        public async Task<APIRespone<Order>> CompleteOrderByShipperAsync(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.Payments)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
                return new APIRespone<Order> { Success = false, Message = "Không tìm thấy đơn hàng" };

            // Cập nhật Payment nếu chưa thanh toán
            var payment = order.Payments?.OrderByDescending(p => p.CreatedAt).FirstOrDefault();
            if (payment != null && payment.PaymentStatus == PaymentStatus.Pending)
            {
                payment.PaymentStatus = PaymentStatus.Paid;
                payment.PaidAt = DateTime.Now;
            }

            // Cập nhật trạng thái đơn hàng
            order.Status = OrderStatus.Completed;

            await _context.SaveChangesAsync();
            // 🚚 Gửi thông báo cho người dùng
            await _notificationRepo.AddAsync(new Notification
            {
                UserId = order.UserId,
                OrderId = order.OrderId,
                Title = "Đơn hàng đã giao thành công",
                Message = $"Đơn hàng #{order.OrderId} của bạn đã được giao thành công.",
                Type = NotificationType.Shipping,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            });

            // 💰 Nếu đơn hàng đã thanh toán
            if (order.Payments?.Any(p => p.PaymentStatus == PaymentStatus.Paid) == true)
            {
                await _notificationRepo.AddAsync(new Notification
                {
                    UserId = order.UserId,
                    OrderId = order.OrderId,
                    Title = "Thanh toán thành công",
                    Message = $"Đơn hàng #{order.OrderId} của bạn đã được thanh toán thành công.",
                    Type = NotificationType.Payment,
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                });
            }

            return new APIRespone<Order>
            {
                Success = true,
                Data = order,
                Message = "Shipper đã giao hàng và thanh toán thành công"
            };
        }

        public async Task<APIRespone<IEnumerable<Order>>> GetAvailableOrdersForShipperAsync()
        {
            var orders = await _context.Orders
                .Include(o => o.Shipping)
                .Include(o => o.Payments)
                .Where(o => o.Status == OrderStatus.Confirmed) // chỉ lấy đơn đã xác nhận, chưa giao
                .ToListAsync();

            return new APIRespone<IEnumerable<Order>>
            {
                Success = true,
                Data = orders,
                Message = "Danh sách đơn hàng có thể nhận giao"
            };
        }
        public async Task<APIRespone<Order>> AssignOrderToShipperAsync(int orderId, int shipperId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
                return new APIRespone<Order> { Success = false, Message = "Không tìm thấy đơn hàng" };

            var shipper = await _context.Users.FindAsync(shipperId);
            if (shipper == null || shipper.Role != UserRole.Shipper)
                return new APIRespone<Order> { Success = false, Message = "Người nhận không hợp lệ" };

            // Nếu đơn đã có shipper thì không cho nhận nữa
            if (order.ShipperId != null && order.ShipperId > 0)
                return new APIRespone<Order> { Success = false, Message = "Đơn hàng đã có shipper khác nhận" };

            // ✅ Cập nhật shipper và trạng thái
            order.ShipperId = shipperId;
            order.Status = OrderStatus.Shipping;

            await _context.SaveChangesAsync();

            return new APIRespone<Order>
            {
                Success = true,
                Data = order,
                Message = $"Shipper (ID: {shipperId}) đã nhận đơn #{orderId}"
            };
        }

        public async Task<APIRespone<IEnumerable<object>>> GetOrdersOfShipperAsync(int shipperId)
        {
            var orders = await _context.Orders
                .Include(o => o.Shipping)
                .Include(o => o.Payments)
                .Where(o => o.ShipperId == shipperId &&
                            (o.Status == OrderStatus.Shipping || o.Status == OrderStatus.Confirmed))
                .Select(o => new
                {
                    o.OrderId,
                    o.TotalAmount,
                    CustomerName = o.Shipping.RecipientName ?? "Ẩn danh",
                    PhoneNumber = o.Shipping.PhoneNumber ?? "Không có",
                    Email = o.Shipping.Email ?? "",
                    Address = o.Shipping.Address ?? "",
                    City = o.Shipping.City ?? "",
                    PostalCode = o.Shipping.PostalCode ?? "",
                    ShippingFee = o.Shipping.ShippingFee,
                    Status = o.Status.ToString(),
                    PaymentStatus = o.Payments
                        .OrderByDescending(p => p.PaymentId)
                        .Select(p => p.PaymentStatus.ToString())
                        .FirstOrDefault() ?? "Chưa thanh toán"
                })
                .ToListAsync();

            return new APIRespone<IEnumerable<object>>
            {
                Success = true,
                Data = orders,
                Message = "Danh sách đơn hàng của shipper"
            };
        }


        public async Task<APIRespone<Order>> CompleteOrderByShipperAsync(int orderId, int shipperId)
        {
            var order = await _context.Orders
                .Include(o => o.Payments)
                .FirstOrDefaultAsync(o => o.OrderId == orderId && o.ShipperId == shipperId);

            if (order == null)
                return new APIRespone<Order> { Success = false, Message = "Không tìm thấy đơn hàng của bạn" };

            if (order.Status != OrderStatus.Shipping)
                return new APIRespone<Order> { Success = false, Message = "Đơn hàng không ở trạng thái giao hàng" };

            // ✅ Cập nhật Payment
            var payment = order.Payments?.OrderByDescending(p => p.CreatedAt).FirstOrDefault();
            if (payment != null)
            {
                payment.PaymentStatus = PaymentStatus.Paid;
                payment.PaidAt = DateTime.Now;
            }

            order.Status = OrderStatus.Completed;
            await _context.SaveChangesAsync();

            return new APIRespone<Order>
            {
                Success = true,
                Data = order,
                Message = $"Shipper #{shipperId} đã hoàn tất giao đơn #{orderId}"
            };
        }

    }
}
