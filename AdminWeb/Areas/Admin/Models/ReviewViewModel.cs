using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace AdminWeb.Areas.Admin.Models
{
    public class ReviewViewModel
    {
        public int ReviewId { get; set; }

        [Display(Name = "Mã sản phẩm")]
        public int ProductId { get; set; }

        [Display(Name = "Mã người dùng")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Đánh giá không được để trống")]
        [Range(1, 5, ErrorMessage = "Đánh giá phải từ 1 đến 5 sao")]
        [Display(Name = "Đánh giá")]
        public int Rating { get; set; }

        [Display(Name = "Bình luận")]
        public string? Comment { get; set; }

        [Display(Name = "Ngày tạo")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Tên sản phẩm")]
        public string? ProductName { get; set; }

        [Display(Name = "Tên người dùng")]
        public string? UserFullName { get; set; }
    }

    public class CreateReviewViewModel
    {
        [Required(ErrorMessage = "Sản phẩm là bắt buộc")]
        [Display(Name = "Sản phẩm")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Người dùng là bắt buộc")]
        [Display(Name = "Người dùng")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Đánh giá không được để trống")]
        [Range(1, 5, ErrorMessage = "Đánh giá phải từ 1 đến 5 sao")]
        [Display(Name = "Đánh giá")]
        public int Rating { get; set; }

        [Display(Name = "Bình luận")]
        public string? Comment { get; set; }

        // Dropdown lists
        public IEnumerable<SelectListItem> ProductList { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> UserList { get; set; } = new List<SelectListItem>();
    }

    public class UpdateReviewViewModel
    {
        public int ReviewId { get; set; }

        [Required(ErrorMessage = "Đánh giá không được để trống")]
        [Range(1, 5, ErrorMessage = "Đánh giá phải từ 1 đến 5 sao")]
        [Display(Name = "Đánh giá")]
        public int Rating { get; set; }

        [Display(Name = "Bình luận")]
        public string? Comment { get; set; }
    }

    public class ReviewSearchViewModel
    {
        [Display(Name = "Tìm kiếm")]
        public string? SearchKeyword { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
