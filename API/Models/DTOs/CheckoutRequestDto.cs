namespace API.Models.DTOs
{
    public class CheckoutRequestDto
    {
        public int UserId { get; set; }
        public List<int> CartIds { get; set; } = new List<int>();

        // Shipping Info
        public string RecipientName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string Address { get; set; } = string.Empty;
        public string? City { get; set; }
        public string? PostalCode { get; set; }

        // Payment Method
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.CashOnDelivery;
    }
}
