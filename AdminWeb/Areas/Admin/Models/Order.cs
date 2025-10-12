using System.ComponentModel.DataAnnotations;

namespace AdminWeb.Areas.Admin.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public DateTime OrderDate { get; set; }
        [Required(ErrorMessage = "Trạng thái đơn hàng là bắt buộc")]
        public OrderStatus Status { get; set; } 
        public decimal TotalAmount { get; set; }
        public int ShippingId { get; set; }

        // Navigation
        public UserViewModel? Users { get; set; }
        public Shipping? Shipping { get; set; }
        public ICollection<OrderDetail>? OrderDetails { get; set; }
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
