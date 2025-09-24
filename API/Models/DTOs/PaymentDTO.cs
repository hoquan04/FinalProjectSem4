using System.ComponentModel.DataAnnotations;

namespace API.Models.DTOs
{
    public class PaymentCreateDto
    {
        [Required]
        public int OrderId { get; set; }

        [Required]
        public PaymentMethod PaymentMethod { get; set; }
    }

    public class PaymentUpdateDto
    {
        [Required]
        public PaymentMethod PaymentMethod { get; set; }

        [Required]
        public PaymentStatus PaymentStatus { get; set; }

        public DateTime? PaidAt { get; set; }
    }
}
