using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    public class Payment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PaymentId { get; set; }

        [Required(ErrorMessage = "Đơn hàng là bắt buộc")]
        public int OrderId { get; set; }

        [Required(ErrorMessage = "Phương thức thanh toán là bắt buộc")]
        public PaymentMethod PaymentMethod { get; set; }

        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;

        public DateTime? PaidAt { get; set; }

        // Navigation
        public Order Order { get; set; }
    }

    public enum PaymentMethod
    {
        CashOnDelivery,
        CreditCard,
        Momo,
        BankTransfer
    }

    public enum PaymentStatus
    {
        Pending,
        Paid,
        Failed
    }
}
