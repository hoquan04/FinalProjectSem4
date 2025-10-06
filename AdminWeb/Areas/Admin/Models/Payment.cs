namespace AdminWeb.Areas.Admin.Models
{
    public class Payment
    {
        public int PaymentId { get; set; }
        public int OrderId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
        public DateTime CreatedAt { get; set; }
        public DateTime? PaidAt { get; set; }

        // Navigation
        public Order? Order { get; set; }
    }

    public enum PaymentMethod
    {
        CashOnDelivery,
        CreditCard,
        Momo,
        BankTransfer,
        VNPay
    }

    public enum PaymentStatus
    {
        Pending,
        Paid,
        Failed
    }
}
