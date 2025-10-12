namespace AdminWeb.Areas.Admin.Models.DTOs
{
    public class OrderDisplayDto
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;

        // 🚚 Thông tin giao hàng
        public string? RecipientName { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
    }
}
