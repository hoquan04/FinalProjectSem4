using API.Data;
using API.Models;
using API.Repositories.IRepositories;
using API.Repositories.RestAPI;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly DataContext _context;

        public NotificationRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<APIRespone<IEnumerable<Notification>>> GetAllAsync()
        {
            var notifications = await _context.Notifications
                .Include(n => n.Order)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            return new APIRespone<IEnumerable<Notification>>
            {
                Success = true,
                Message = "Lấy danh sách thông báo thành công",
                Data = notifications
            };
        }

        public async Task<APIRespone<Notification>> GetByIdAsync(int id)
        {
            var notification = await _context.Notifications
                .Include(n => n.Order)
                .FirstOrDefaultAsync(n => n.NotificationId == id);

            if (notification == null)
            {
                return new APIRespone<Notification>
                {
                    Success = false,
                    Message = "Không tìm thấy thông báo",
                    Data = null
                };
            }

            return new APIRespone<Notification>
            {
                Success = true,
                Message = "Lấy thông báo thành công",
                Data = notification
            };
        }

        public async Task<APIRespone<Notification>> AddAsync(Notification entity)
        {
            entity.CreatedAt = DateTime.UtcNow;
            _context.Notifications.Add(entity);
            await _context.SaveChangesAsync();

            return new APIRespone<Notification>
            {
                Success = true,
                Message = "Thêm thông báo thành công",
                Data = entity
            };
        }

        public async Task<APIRespone<Notification>> UpdateAsync(int id, Notification entity)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null)
            {
                return new APIRespone<Notification>
                {
                    Success = false,
                    Message = "Không tìm thấy thông báo",
                    Data = null
                };
            }

            notification.Title = entity.Title;
            notification.Message = entity.Message;
            notification.Type = entity.Type;
            notification.IsRead = entity.IsRead;

            await _context.SaveChangesAsync();

            return new APIRespone<Notification>
            {
                Success = true,
                Message = "Cập nhật thông báo thành công",
                Data = notification
            };
        }

        public async Task<APIRespone<bool>> DeleteAsync(int id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null)
            {
                return new APIRespone<bool>
                {
                    Success = false,
                    Message = "Không tìm thấy thông báo",
                    Data = false
                };
            }

            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();

            return new APIRespone<bool>
            {
                Success = true,
                Message = "Xóa thông báo thành công",
                Data = true
            };
        }

        public async Task<APIRespone<PagedResponse<Notification>>> GetPageAsync(int pageNow, int pageSize)
        {
            var query = _context.Notifications
                .Include(n => n.Order)
                .OrderByDescending(n => n.CreatedAt)
                .AsQueryable();

            var totalCount = await query.CountAsync();
            var totalPage = (int)Math.Ceiling(totalCount / (double)pageSize);

            var data = await query
                .Skip((pageNow - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var response = new PagedResponse<Notification>
            {
                Data = data,
                PageNow = pageNow,
                PageSize = pageSize,
                TotalPage = totalPage,
                TotalCount = totalCount
            };

            return new APIRespone<PagedResponse<Notification>>
            {
                Success = true,
                Message = "Lấy danh sách thông báo phân trang thành công",
                Data = response
            };
        }

        public async Task<APIRespone<List<Notification>>> GetByUserIdAsync(int userId)
        {
            var notifications = await _context.Notifications
                .Include(n => n.Order)
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            return new APIRespone<List<Notification>>
            {
                Success = true,
                Message = "Lấy thông báo của người dùng thành công",
                Data = notifications
            };
        }

        public async Task<APIRespone<bool>> MarkAsReadAsync(int notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification == null)
            {
                return new APIRespone<bool>
                {
                    Success = false,
                    Message = "Không tìm thấy thông báo",
                    Data = false
                };
            }

            notification.IsRead = true;
            await _context.SaveChangesAsync();

            return new APIRespone<bool>
            {
                Success = true,
                Message = "Đánh dấu đã đọc thành công",
                Data = true
            };
        }

        public async Task<APIRespone<bool>> MarkAllAsReadAsync(int userId)
        {
            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            foreach (var notification in notifications)
            {
                notification.IsRead = true;
            }

            await _context.SaveChangesAsync();

            return new APIRespone<bool>
            {
                Success = true,
                Message = $"Đã đánh dấu {notifications.Count} thông báo là đã đọc",
                Data = true
            };
        }

        public async Task<APIRespone<int>> GetUnreadCountAsync(int userId)
        {
            var count = await _context.Notifications
                .CountAsync(n => n.UserId == userId && !n.IsRead);

            return new APIRespone<int>
            {
                Success = true,
                Message = "Lấy số thông báo chưa đọc thành công",
                Data = count
            };
        }

        public async Task<APIRespone<bool>> DeleteAllByUserIdAsync(int userId)
        {
            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId)
                .ToListAsync();

            _context.Notifications.RemoveRange(notifications);
            await _context.SaveChangesAsync();

            return new APIRespone<bool>
            {
                Success = true,
                Message = $"Đã xóa {notifications.Count} thông báo",
                Data = true
            };
        }
    }
}
