using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    [NotMapped]
    public class Cart
    {
        [Key]
        public int CartId { get; set; }

        [Required(ErrorMessage = "Người dùng là bắt buộc")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Sản phẩm là bắt buộc")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Số lượng không được để trống")]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public int Quantity { get; set; } = 1;

        public DateTime AddedAt { get; set; } = DateTime.Now;

        // Navigation (có thể lấy từ DB khi cần)
        public User Users { get; set; }
        public Product Product { get; set; }
    }
}
