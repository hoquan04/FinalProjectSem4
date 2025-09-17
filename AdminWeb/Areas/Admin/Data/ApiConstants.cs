namespace AdminWeb.Areas.Admin.Data
{
    public class ApiConstants
    {

        // Dev: API & AdminWeb chạy cùng máy
        public const string Domain = "localhost";
        public const string Port = "7245";


        public static string BaseUrl => $"https://{Domain}:{Port}/api/";
        public static string CategoryApi => $"{BaseUrl}category";
        public static string UserApi => $"{BaseUrl}user";
        public static string BrandApi => $"{BaseUrl}brand";
        public static string ProductApi => $"{BaseUrl}product";
        public static string OrderApi => $"{BaseUrl}order";
        public static string OrderDetailApi => $"{BaseUrl}orderdetail";
        public static string AccountApi => $"{BaseUrl}accounts";

        public static string AuthApi => $"{BaseUrl}auth";
        public static string AuthLogin => $"{AuthApi}/login";     // hoặc /login-admin
        public static string AuthRegister => $"{AuthApi}/register";

        public static string ReviewApi => $"{BaseUrl}Review"; // Thêm dòng này

    }
}
