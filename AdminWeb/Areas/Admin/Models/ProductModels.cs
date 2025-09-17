using System.ComponentModel.DataAnnotations;

namespace AdminWeb.Areas.Admin.Models
{
    /// <summary>
    /// Model để hiển thị Product (từ API GET)
    /// </summary>
    public class ProductViewModel
    {
        public int ProductId { get; set; }
        
        [Display(Name = "Danh mục")]
        public int CategoryId { get; set; }
        
        [Display(Name = "Tên sản phẩm")]
        public string Name { get; set; } = string.Empty;
        
        [Display(Name = "Mô tả")]
        public string? Description { get; set; }
        
        [Display(Name = "Giá")]
        public decimal Price { get; set; }
        
        [Display(Name = "Số lượng tồn kho")]
        public int StockQuantity { get; set; }
        
        [Display(Name = "Hình ảnh")]
        public string? ImageUrl { get; set; }
        
        [Display(Name = "Ngày tạo")]
        public DateTime CreatedAt { get; set; }
        
        // Navigation
        public CategoryViewModel? Category { get; set; }
    }

    /// <summary>
    /// Model để tạo mới Product (gửi lên API POST)
    /// </summary>
    public class ProductCreateModel
    {
        [Required(ErrorMessage = "Danh mục sản phẩm là bắt buộc")]
        [Display(Name = "Danh mục")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Tên sản phẩm không được để trống")]
        [StringLength(150, ErrorMessage = "Tên sản phẩm tối đa 150 ký tự")]
        [Display(Name = "Tên sản phẩm")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Mô tả")]
        [StringLength(1000, ErrorMessage = "Mô tả tối đa 1000 ký tự")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Giá sản phẩm không được để trống")]
        [Range(0, 9999999999.99, ErrorMessage = "Giá sản phẩm phải lớn hơn hoặc bằng 0")]
        [Display(Name = "Giá")]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Số lượng tồn kho không hợp lệ")]
        [Display(Name = "Số lượng tồn kho")]
        public int StockQuantity { get; set; } = 0;

        [StringLength(255, ErrorMessage = "Đường dẫn ảnh tối đa 255 ký tự")]
        [Display(Name = "Hình ảnh")]
        public string? ImageUrl { get; set; }
    }

    /// <summary>
    /// Model để chỉnh sửa Product (gửi lên API PUT)
    /// </summary>
    public class ProductEditModel
    {
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Danh mục sản phẩm là bắt buộc")]
        [Display(Name = "Danh mục")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Tên sản phẩm không được để trống")]
        [StringLength(150, ErrorMessage = "Tên sản phẩm tối đa 150 ký tự")]
        [Display(Name = "Tên sản phẩm")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Mô tả")]
        [StringLength(1000, ErrorMessage = "Mô tả tối đa 1000 ký tự")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Giá sản phẩm không được để trống")]
        [Range(0, 9999999999.99, ErrorMessage = "Giá sản phẩm phải lớn hơn hoặc bằng 0")]
        [Display(Name = "Giá")]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Số lượng tồn kho không hợp lệ")]
        [Display(Name = "Số lượng tồn kho")]
        public int StockQuantity { get; set; }

        [StringLength(255, ErrorMessage = "Đường dẫn ảnh tối đa 255 ký tự")]
        [Display(Name = "Hình ảnh")]
        public string? ImageUrl { get; set; }
    }

    /// <summary>
    /// Model để search products
    /// </summary>
    public class ProductSearchModel
    {
        [Display(Name = "Từ khóa tìm kiếm")]
        public string? SearchTerm { get; set; }
        
        [Display(Name = "Danh mục")]
        public int? CategoryId { get; set; }
        
        [Display(Name = "Trang hiện tại")]
        public int PageNow { get; set; } = 1;
        
        [Display(Name = "Số bản ghi mỗi trang")]
        public int PageSize { get; set; } = 10;
    }
}   
