using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    public class Shipping
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ShippingId { get; set; }

        [Required(ErrorMessage = "Địa chỉ giao hàng không được để trống")]
        public string Address { get; set; }

        [StringLength(100, ErrorMessage = "Tên thành phố tối đa 100 ký tự")]
        public string? City { get; set; }

        [StringLength(20, ErrorMessage = "Mã bưu điện tối đa 20 ký tự")]
        public string? PostalCode { get; set; }

        [Range(0, 9999999999.99, ErrorMessage = "Phí vận chuyển phải lớn hơn hoặc bằng 0")]
        public decimal? ShippingFee { get; set; }

        [Range(1, 365, ErrorMessage = "Số ngày giao hàng ước tính phải từ 1 đến 365")]
        public int? EstimatedDays { get; set; }

        // Navigation
        public ICollection<Order>? Orders { get; set; }
    }
}
