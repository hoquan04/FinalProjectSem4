namespace AdminWeb.Areas.Admin.Models
{
    public class Shipping
    {
        public int ShippingId { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? PostalCode { get; set; }
        public decimal? ShippingFee { get; set; }
        public int? EstimatedDays { get; set; }

        // Navigation
        public ICollection<Order>? Orders { get; set; }
    }
}
