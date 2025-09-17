using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace API.Models
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Danh mục sản phẩm là bắt buộc")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Tên sản phẩm không được để trống")]
        [StringLength(150, ErrorMessage = "Tên sản phẩm tối đa 150 ký tự")]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required(ErrorMessage = "Giá sản phẩm không được để trống")]
        [Range(0, 9999999999.99, ErrorMessage = "Giá sản phẩm phải lớn hơn hoặc bằng 0")]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Số lượng tồn kho không hợp lệ")]
        public int StockQuantity { get; set; } = 0;

        [StringLength(255, ErrorMessage = "Đường dẫn ảnh tối đa 255 ký tự")]
        public string? ImageUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation Properties - Thêm JsonIgnore để tránh cycles
        public Category? Category { get; set; }
        
        [JsonIgnore]
        public ICollection<OrderDetail>? OrderDetails { get; set; }
        
        [JsonIgnore]
        public ICollection<Review>? Reviews { get; set; }
    }
}
