namespace AdminWeb.Areas.Admin.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatus Status { get; set; }
        public decimal TotalAmount { get; set; }
        public int ShippingId { get; set; }

        // Navigation
        public User? Users { get; set; }
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
