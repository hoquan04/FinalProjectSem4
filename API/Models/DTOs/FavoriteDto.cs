using System.ComponentModel.DataAnnotations;

namespace API.Models.DTOs
{
    public class FavoriteDto
    {
        public class AddFavoriteDto
        {
            [Required(ErrorMessage = "Người dùng là bắt buộc")]
            public int UserId { get; set; }

            [Required(ErrorMessage = "Sản phẩm là bắt buộc")]
            public int ProductId { get; set; }
        }

        public class FavoriteResponseDto
        {
            public int FavoriteId { get; set; }
            public int UserId { get; set; }
            public int ProductId { get; set; }
            public DateTime CreatedAt { get; set; }

            // Thông tin sản phẩm
            public string? ProductName { get; set; }
            public string? ProductDescription { get; set; }
            public decimal ProductPrice { get; set; }
            public string? ProductImageUrl { get; set; }
            public int ProductStockQuantity { get; set; }
            public string? CategoryName { get; set; }
        }

        public class FavoriteCheckDto
        {
            public int UserId { get; set; }
            public int ProductId { get; set; }
            public bool IsFavorite { get; set; }
        }
    }
}
