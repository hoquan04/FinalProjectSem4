using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Routing;

namespace AdminWeb.Areas.Admin.Filters
{
    public class AdminAuthorizeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var http = context.HttpContext;
            var isLogged = http.Session.GetString("Auth:IsLoggedIn") == "true";

            if (!isLogged)
            {
                var urlHelper = new UrlHelper(context);
                var returnUrl = http.Request.Path + http.Request.QueryString;
                context.Result = new RedirectToActionResult("Login", "Account", new { area = "Admin", returnUrl });
                return;
            }

            // Nếu bạn muốn bắt buộc Role = Admin:
            // var role = http.Session.GetString("Auth:Role");
            // if (!string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase))
            // {
            //     context.Result = new ContentResult { Content = "Bạn không có quyền truy cập.", StatusCode = 403 };
            //     return;
            // }
        }
    }
}
