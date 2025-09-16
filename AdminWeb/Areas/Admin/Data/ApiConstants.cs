namespace AdminWeb.Areas.Admin.Data
{
    public class ApiConstants
    {
        //ip quân
        public const string Domain = "localhost";
      
        public const string Port = "7245";

        public static string BaseUrl => $"http://{Domain}:{Port}/api/";
        public static string CategoryApi => $"{BaseUrl}category";
        public static string BrandApi => $"{BaseUrl}brand";

        public static string ProductApi => $"{BaseUrl}product";
        public static string OrderApi => $"{BaseUrl}order";
        public static string OrderDetailApi => $"{BaseUrl}orderdetail";
        public static string AccountApi => $"{BaseUrl}accounts";
        public static string FileUploadApi => $"{BaseUrl}file/upload";
        public static string FileDeleteApi => $"{BaseUrl}file/delete";
    }
}
