using System.ComponentModel.DataAnnotations;

namespace AdminWeb.Areas.Admin.Models
{
    /// <summary>
    /// Model ?? hi?n th? Category (t? API GET)
    /// </summary>
    public class CategoryViewModel
    {
        public int CategoryId { get; set; }
        
        [Display(Name = "Tên danh m?c")]
        public string Name { get; set; } = string.Empty;
        
        [Display(Name = "Mô t?")]
        public string? Description { get; set; }
    }

    /// <summary>
    /// Model ?? t?o m?i Category (g?i lên API POST)
    /// </summary>
    public class CategoryCreateModel
    {
        [Required(ErrorMessage = "Tên danh m?c không ???c ?? tr?ng")]
        [StringLength(100, ErrorMessage = "Tên danh m?c t?i ?a 100 ký t?")]
        [Display(Name = "Tên danh m?c")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Mô t?")]
        [StringLength(500, ErrorMessage = "Mô t? t?i ?a 500 ký t?")]
        public string? Description { get; set; }
    }

    /// <summary>
    /// Model ?? ch?nh s?a Category (g?i lên API PUT)
    /// </summary>
    public class CategoryEditModel
    {
        public int CategoryId { get; set; }
        
        [Required(ErrorMessage = "Tên danh m?c không ???c ?? tr?ng")]
        [StringLength(100, ErrorMessage = "Tên danh m?c t?i ?a 100 ký t?")]
        [Display(Name = "Tên danh m?c")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Mô t?")]
        [StringLength(500, ErrorMessage = "Mô t? t?i ?a 500 ký t?")]
        public string? Description { get; set; }
    }
}