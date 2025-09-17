using System.ComponentModel.DataAnnotations;

namespace AdminWeb.Areas.Admin.Models
{
    /// <summary>
    /// Model để hiển thị Category (từ API GET)
    /// </summary>
    public class CategoryViewModel
    {
        public int CategoryId { get; set; }
        
        [Display(Name = "Tên danh mục")]
        public string Name { get; set; } = string.Empty;
        
        [Display(Name = "Mô tả")]
        public string? Description { get; set; }
    }

    /// <summary>
    /// Model để tạo mới Category (gửi lên API POST)
    /// </summary>
    public class CategoryCreateModel
    {
        [Required(ErrorMessage = "Tên danh mục không được để trống")]
        [StringLength(100, ErrorMessage = "Tên danh mục tối đa 100 ký tự")]
        [Display(Name = "Tên danh mục")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Mô tả")]
        [StringLength(500, ErrorMessage = "Mô tả tối đa 500 ký tự")]
        public string? Description { get; set; }
    }

    /// <summary>
    /// Model để chỉnh sửa Category (gửi lên API PUT)
    /// </summary>
    public class CategoryEditModel
    {
        public int CategoryId { get; set; }
        
        [Required(ErrorMessage = "Tên danh mục không được để trống")]
        [StringLength(100, ErrorMessage = "Tên danh mục tối đa 100 ký tự")]
        [Display(Name = "Tên danh mục")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Mô tả")]
        [StringLength(500, ErrorMessage = "Mô tả tối đa 500 ký tự")]
        public string? Description { get; set; }
    }
}