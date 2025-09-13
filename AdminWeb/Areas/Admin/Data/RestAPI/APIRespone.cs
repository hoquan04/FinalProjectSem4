namespace AdminWeb.Areas.Admin.Data.RestAPI
{
    public class APIRespone<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
    }
}
