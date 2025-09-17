namespace AdminWeb.Models
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        // ? Th�m thu?c t�nh n�y
        public string? Message { get; set; }
    }
}
