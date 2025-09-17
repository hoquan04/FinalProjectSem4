namespace AdminWeb.Areas.Admin.Data.Services
{
    public class JwtSessionHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _http;
        public JwtSessionHandler(IHttpContextAccessor http) => _http = http;

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage req, CancellationToken ct)
        {
            var token = _http.HttpContext?.Session.GetString("JWT_TOKEN");
            if (!string.IsNullOrWhiteSpace(token) && !req.Headers.Contains("Authorization"))
            {
                req.Headers.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
            return base.SendAsync(req, ct);
        }
    }
}
