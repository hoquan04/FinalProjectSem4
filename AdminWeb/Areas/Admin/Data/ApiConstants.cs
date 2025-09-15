namespace AdminWeb.Areas.Admin.Data
{
    public class ApiConstants
    {
        //ip quân
        public const string Domain = "localhost";
      
        public const string Port = "50803";

        public static string BaseUrl => $"https://{Domain}:{Port}/api/";
        public static string CategoryApi => $"{BaseUrl}category";
        public static string BrandApi => $"{BaseUrl}brand";

        public static string ProductApi => $"{BaseUrl}product";
        public static string OrderApi => $"{BaseUrl}order";
        public static string AccountApi => $"{BaseUrl}accounts";
        public static string ReviewApi => $"{BaseUrl}Review"; // Thêm dòng này
    }
}
