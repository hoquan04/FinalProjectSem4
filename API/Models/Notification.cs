using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace API.Models
{
    public class Notification
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int NotificationId { get; set; }

        [Required(ErrorMessage = "Người dùng là bắt buộc")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Tiêu đề thông báo không được để trống")]
        [StringLength(200, ErrorMessage = "Tiêu đề tối đa 200 ký tự")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nội dung thông báo không được để trống")]
        [StringLength(1000, ErrorMessage = "Nội dung tối đa 1000 ký tự")]
        public string Message { get; set; } = string.Empty;

        [Required(ErrorMessage = "Loại thông báo là bắt buộc")]
        public NotificationType Type { get; set; } = NotificationType.Order;

        // Khóa ngoại đến Order (nullable vì có thể có thông báo không liên quan đến order)
        public int? OrderId { get; set; }

        // Trạng thái đã đọc
        public bool IsRead { get; set; } = false;

        // Thời gian tạo thông báo
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Thời gian đọc thông báo
        public DateTime? ReadAt { get; set; }

        // Navigation
        [JsonIgnore]
        public User? User { get; set; }

        [JsonIgnore]
        public Order? Order { get; set; }
    }

    public enum NotificationType
    {
        RoleUpdate,
        Order,          // Thông báo về đơn hàng
        Payment,        // Thông báo về thanh toán
        Shipping,       // Thông báo về vận chuyển
        System,         // Thông báo hệ thống
        Promotion       // Thông báo khuyến mãi
    }
}
