using System.ComponentModel.DataAnnotations;

namespace API.Models.DTOs
{
    public class ReviewDto
    {
        public class CreateReviewDto
        {
            [Required(ErrorMessage = "Sản phẩm là bắt buộc")]
            public int ProductId { get; set; }

            [Required(ErrorMessage = "Người dùng là bắt buộc")]
            public int UserId { get; set; }

            [Required(ErrorMessage = "Đánh giá không được để trống")]
            [Range(1, 5, ErrorMessage = "Đánh giá phải từ 1 đến 5 sao")]
            public int Rating { get; set; }

            public string? Comment { get; set; }
        }

        public class UpdateReviewDto
        {
            [Required(ErrorMessage = "Đánh giá không được để trống")]
            [Range(1, 5, ErrorMessage = "Đánh giá phải từ 1 đến 5 sao")]
            public int Rating { get; set; }

            public string? Comment { get; set; }
        }

        public class ReviewResponseDto
        {
            public int ReviewId { get; set; }
            public int ProductId { get; set; }
            public int UserId { get; set; }
            public int Rating { get; set; }
            public string? Comment { get; set; }
            public DateTime CreatedAt { get; set; }

            // Thông tin bổ sung
            public string? ProductName { get; set; }
            public string? UserFullName { get; set; }
        }
    }
}
