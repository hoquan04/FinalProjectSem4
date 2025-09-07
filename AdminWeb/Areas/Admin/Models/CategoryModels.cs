using System.ComponentModel.DataAnnotations;

namespace AdminWeb.Areas.Admin.Models
{
    /// <summary>
    /// Model ?? hi?n th? Category (t? API GET)
    /// </summary>
    public class CategoryViewModel
    {
        public int CategoryId { get; set; }
        
        [Display(Name = "T�n danh m?c")]
        public string Name { get; set; } = string.Empty;
        
        [Display(Name = "M� t?")]
        public string? Description { get; set; }
    }

    /// <summary>
    /// Model ?? t?o m?i Category (g?i l�n API POST)
    /// </summary>
    public class CategoryCreateModel
    {
        [Required(ErrorMessage = "T�n danh m?c kh�ng ???c ?? tr?ng")]
        [StringLength(100, ErrorMessage = "T�n danh m?c t?i ?a 100 k� t?")]
        [Display(Name = "T�n danh m?c")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "M� t?")]
        [StringLength(500, ErrorMessage = "M� t? t?i ?a 500 k� t?")]
        public string? Description { get; set; }
    }

    /// <summary>
    /// Model ?? ch?nh s?a Category (g?i l�n API PUT)
    /// </summary>
    public class CategoryEditModel
    {
        public int CategoryId { get; set; }
        
        [Required(ErrorMessage = "T�n danh m?c kh�ng ???c ?? tr?ng")]
        [StringLength(100, ErrorMessage = "T�n danh m?c t?i ?a 100 k� t?")]
        [Display(Name = "T�n danh m?c")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "M� t?")]
        [StringLength(500, ErrorMessage = "M� t? t?i ?a 500 k� t?")]
        public string? Description { get; set; }
    }
}