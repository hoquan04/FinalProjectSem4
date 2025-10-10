using System.ComponentModel.DataAnnotations;

namespace API.Models.DTOs
{
    // DTO cho response GetAll / GetById
    public class ShippingResponseDto
    {
        public int ShippingId { get; set; }
        public string RecipientName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string Address { get; set; } = string.Empty;
        public string? City { get; set; }
        public string? PostalCode { get; set; }
        public decimal? ShippingFee { get; set; }
        public int? EstimatedDays { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    // DTO cho Create
    public class ShippingCreateDto
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

    // DTO cho Update
    public class ShippingUpdateDto
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

    // DTO cho tìm kiếm
    public class ShippingSearchDto
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
