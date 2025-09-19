using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

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

        // Thời điểm tạo payment (dù thành công, pending hay fail đều có)  lưu ý không được dùng làm input để thêm mới hoặc cập nhật nó phải ẩn đi
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Thời điểm thanh toán thành công (chỉ set khi PaymentStatus = Paid) nó là lịch sử ngày, khi thay đổi trạng thái thanh toán
        public DateTime? PaidAt { get; set; }

        // Navigation
 
        public Order? Order { get; set; }
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
