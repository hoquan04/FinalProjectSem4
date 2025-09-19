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

        // Navigation
   
        public User? Users { get; set; }
       
        public Shipping? Shipping { get; set; }

        [JsonIgnore]
        public ICollection<OrderDetail>? OrderDetails { get; set; }
        [JsonIgnore]
        public ICollection<Payment>? Payments { get; set; }
    }

    public enum OrderStatus
    {
        Pending,
        Confirmed,
        Shipping,
        Completed,
        Cancelled
    }
}
