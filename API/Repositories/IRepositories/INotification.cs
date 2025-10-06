using API.Models;
using API.Repositories.RestAPI;

namespace API.Repositories.IRepositories
{
    public interface INotificationRepository : IType<Notification>
    {
        Task<APIRespone<List<Notification>>> GetByUserIdAsync(int userId);
        Task<APIRespone<bool>> MarkAsReadAsync(int notificationId);
        Task<APIRespone<bool>> MarkAllAsReadAsync(int userId);
        Task<APIRespone<int>> GetUnreadCountAsync(int userId);
        Task<APIRespone<bool>> DeleteAllByUserIdAsync(int userId);
    }
}
