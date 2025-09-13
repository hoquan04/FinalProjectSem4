namespace AdminWeb.Areas.Admin.Data.RestAPI
{
    public class PagedResponse<T>
    {
        public IEnumerable<T>? Data { get; set; }
        public int PageNow { get; set; }
        public int PageSize { get; set; }
        public int TotalPage { get; set; }
        public int TotalCount { get; set; }
    }
}
