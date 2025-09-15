namespace AdminWeb.Models
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        // ? Thêm thu?c tính này
        public string? Message { get; set; }
    }
}
