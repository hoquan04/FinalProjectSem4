namespace AdminWeb.Areas.Admin.Data
{
    public class ApiConstants
    {
        //ip quân
        public const string Domain = "192.168.1.36";
      
        public const string Port = "7245";

        public static string BaseUrl => $"http://{Domain}:{Port}/api/";
        public static string CategoryApi => $"{BaseUrl}category";
        public static string BrandApi => $"{BaseUrl}brand";

        public static string ProductApi => $"{BaseUrl}product";
        public static string OrderApi => $"{BaseUrl}order";
        public static string AccountApi => $"{BaseUrl}accounts";
    }
}
