namespace API.Models.DTOs
{
    public class OrderDetailDto
    {
        public int OrderDetailId { get; set; }
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? ImageUrl { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal SubTotal { get; set; }
        public DateTime CreatedDate { get; set; }

        // Thông tin Order
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }

        // Thông tin Shipping
        public string? CustomerName { get; set; }   // RecipientName
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? PostalCode { get; set; }
        public decimal? ShippingFee { get; set; }

        public OrderStatus Status { get; set; }

        public PaymentStatus? PaymentStatus { get; set; }
    }
}
