using System.ComponentModel.DataAnnotations;
namespace AdminWeb.Areas.Admin.Models
{
    /// <summary>
    /// Model mapping với APIRespone<T> từ API backend
    /// </summary>
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
    }

    /// <summary>
    /// Model mapping với PagedResponse<T> từ API backend
    /// </summary>
    public class PagedResponse<T>
    {
        public List<T> Data { get; set; } = new List<T>();
        public int PageNow { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalPage { get; set; } = 1;
        public int TotalCount { get; set; } = 0;
    }

    /// <summary>
    /// Model để search categories
    /// </summary>
    public class CategorySearchModel
    {
        [Display(Name = "Từ khóa tìm kiếm")]
        public string? SearchTerm { get; set; }
        
        [Display(Name = "Trang hiện tại")]
        public int PageNow { get; set; } = 1;
        
        [Display(Name = "Số bản ghi mỗi trang")]
        public int PageSize { get; set; } = 10;
    }
}
