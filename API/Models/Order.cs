using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace API.Models
{
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderId { get; set; }

        [Required(ErrorMessage = "Người dùng là bắt buộc")]
        public int UserId { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Trạng thái đơn hàng là bắt buộc")]
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        [Required(ErrorMessage = "Tổng tiền không được để trống")]
        [Range(0, 9999999999.99, ErrorMessage = "Tổng tiền phải lớn hơn hoặc bằng 0")]
        public decimal TotalAmount { get; set; }

        public int ShippingId { get; set; }

        // 🔹 Navigation Properties
        [ForeignKey(nameof(UserId))]
        [JsonIgnore]
        public User? Users { get; set; }   // ✅ Cho phép null

        [ForeignKey(nameof(ShippingId))]
        public Shipping? Shipping { get; set; }  // ✅ Cho phép null


        [JsonIgnore]
        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>(); // ✅ không bao giờ null

        [JsonIgnore]
        public ICollection<Payment> Payments { get; set; } = new List<Payment>(); // ✅ không null

        [JsonIgnore]
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>(); // ✅ không null

        // 🚚 Shipper có thể null
        public int? ShipperId { get; set; }

        [ForeignKey(nameof(ShipperId))]
        [JsonIgnore]
        public User? Shipper { get; set; }
    }

    public enum OrderStatus
    {
        Pending,     // Đang chờ xác nhận
        Confirmed,   // Đã xác nhận
        Shipping,    // Đang giao
        Completed,   // Hoàn tất
        Cancelled    // Đã hủy
    }
}
