namespace API.Models.DTOs
{
    public class SearchOrderDetail
    {
        // Từ khóa chung (có thể tìm theo ProductName, OrderCode, v.v.)
        public string? Keyword { get; set; }

        // Lọc theo OrderId
        public int? OrderId { get; set; }

        // Lọc theo ProductId
        public int? ProductId { get; set; }

        // Lọc theo khoảng số lượng
        public int? MinQuantity { get; set; }
        public int? MaxQuantity { get; set; }

        // Lọc theo khoảng giá (UnitPrice hoặc SubTotal)
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

        // Lọc theo ngày tạo order detail (nếu có cột CreatedDate)
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
