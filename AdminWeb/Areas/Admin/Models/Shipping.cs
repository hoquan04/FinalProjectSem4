using System.ComponentModel.DataAnnotations;

namespace AdminWeb.Areas.Admin.Models
{
    public class Shipping
    {
        public int ShippingId { get; set; }

        // 👤 Người nhận
        public string RecipientName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? Email { get; set; }

        // 📍 Địa chỉ
        public string Address { get; set; } = string.Empty;
        public string? City { get; set; }
        public string? PostalCode { get; set; }

        // 💰 Vận chuyển
        public decimal? ShippingFee { get; set; }
        public int? EstimatedDays { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ShippingCreateModel
    {
        [Required, StringLength(150)]
        public string RecipientName { get; set; } = string.Empty;

        [Required, Phone, StringLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        [EmailAddress, StringLength(100)]
        public string? Email { get; set; }

        [Required]
        public string Address { get; set; } = string.Empty;

        [StringLength(100)]
        public string? City { get; set; }

        [StringLength(20)]
        public string? PostalCode { get; set; }

        [Range(0, 9999999999.99)]
        public decimal? ShippingFee { get; set; }

        [Range(1, 365)]
        public int? EstimatedDays { get; set; }
    }

    public class ShippingEditModel : ShippingCreateModel
    {
        public int ShippingId { get; set; }
    }

    public class ShippingSearchModel
    {
        public string? RecipientName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? PostalCode { get; set; }

        public int PageNow { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

}
