using System.ComponentModel.DataAnnotations;

namespace API.Models.DTOs
{
    // DTO cho response GetAll - hiển thị thông tin User từ Order
    public class ShippingResponseDto
    {
        public int ShippingId { get; set; }
        public string Address { get; set; }
        public string? City { get; set; }
        public string? PostalCode { get; set; }
        public decimal? ShippingFee { get; set; }
        public int? EstimatedDays { get; set; }
    }
    

    // DTO cho Create
    public class ShippingCreateDto
    {
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
    }

    // DTO cho Update
    public class ShippingUpdateDto
    {
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
    }

    // DTO cho tìm kiếm
    public class ShippingSearchDto
    {
        public string? PostalCode { get; set; }
        public string? Address { get; set; }
        public int? OrderId { get; set; }
        public int PageNow { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
