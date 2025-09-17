namespace AdminWeb.Areas.Admin.Models.DTOs
{
    public class SearchOrderDetail
    {
        public string? Keyword { get; set; }
        public int? OrderId { get; set; }
        public int? ProductId { get; set; }
        public int? MinQuantity { get; set; }
        public int? MaxQuantity { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
