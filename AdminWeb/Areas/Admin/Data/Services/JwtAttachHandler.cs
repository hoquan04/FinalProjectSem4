using System.Net.Http.Headers;

namespace AdminWeb.Areas.Admin.Data.Services
{
    public class JwtAttachHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public JwtAttachHandler(IHttpContextAccessor accessor) => _httpContextAccessor = accessor;

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var path = request.RequestUri!.AbsolutePath.ToLowerInvariant();

            // ❌ Chỉ *không* gắn token cho 3 endpoint dưới
            var skipAuth =
                path.EndsWith("/auth/login") ||
                path.EndsWith("/auth/login-admin") ||
                path.EndsWith("/auth/register");

            if (!skipAuth)
            {
                var token = _httpContextAccessor.HttpContext?.Session.GetString("JWT_TOKEN");
                if (!string.IsNullOrEmpty(token))
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}
