namespace AdminWeb.Areas.Admin.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public int CategoryId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; } = 0;
        public string? ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation
        //public Category? Category { get; set; }
        public ICollection<OrderDetail>? OrderDetails { get; set; }
        //public ICollection<Review>? Reviews { get; set; }
    }
}
