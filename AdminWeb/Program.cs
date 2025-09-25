using AdminWeb.Areas.Admin.Data.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;


var builder = WebApplication.CreateBuilder(args);

// Cấu hình encoding UTF-8 cho console
Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();

builder.Services.AddHttpClient<IReviewApiService, ReviewApiService>();
builder.Services.AddScoped<IReviewApiService, ReviewApiService>();
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
// Cấu hình HttpClient với timeout và retry policy
builder.Services.AddHttpClient("APIClient", client =>
{
    client.DefaultRequestHeaders.Accept.Add(
        new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
    client.Timeout = TimeSpan.FromSeconds(30);
    client.BaseAddress = new Uri("http://localhost:7245/");
});

// Các service dùng HttpClient
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<OrderDetailService>();

// Đăng ký Services
builder.Services.AddHttpClient<CategoryService>();
builder.Services.AddScoped<CategoryService>();

builder.Services.AddHttpClient<ProductService>();
builder.Services.AddScoped<ProductService>();

builder.Services.AddHttpClient<PaymentService>();

// Cấu hình localization cho tiếng Việt
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[] { "vi-VN", "en-US" };
    options.SetDefaultCulture("vi-VN")
           .AddSupportedCultures(supportedCultures)
           .AddSupportedUICultures(supportedCultures);
});

// Session
builder.Services.AddSession(o =>
{
    o.Cookie.Name = ".AdminWeb.Session";
    o.IdleTimeout = TimeSpan.FromHours(8);
    o.Cookie.HttpOnly = true;
    o.Cookie.IsEssential = true;
});

// Cookie auth
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(o =>
    {
        o.LoginPath = "/Admin/Account/Login";
        o.AccessDeniedPath = "/Admin/Account/AccessDenied";
        o.ExpireTimeSpan = TimeSpan.FromHours(8);
        o.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();

// HttpClient & handler
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<JwtAttachHandler>();

var apiBase = builder.Configuration["Api:Base"]
             ?? throw new InvalidOperationException("Missing Api:Base (e.g. http://localhost:7245/api/)");

// AuthService: KHÔNG gắn bearer
builder.Services.AddHttpClient<AuthService>(c =>
{
    c.BaseAddress = new Uri(apiBase);
});

// CategoryService: CÓ gắn bearer
builder.Services.AddHttpClient<CategoryService>((sp, c) =>
{
    c.BaseAddress = new Uri(apiBase);
}).AddHttpMessageHandler<JwtAttachHandler>();

// ProfileService: CÓ gắn bearer (đã sửa key và thêm handler)
builder.Services.AddHttpClient<ProfileService>((sp, client) =>
{
    client.BaseAddress = new Uri(apiBase);
    client.Timeout = TimeSpan.FromSeconds(30);
}).AddHttpMessageHandler<JwtAttachHandler>();

// UserService: CÓ gắn bearer
builder.Services.AddHttpClient<UserService>((sp, c) =>
{
    c.BaseAddress = new Uri(apiBase);
}).AddHttpMessageHandler<JwtAttachHandler>();

// (Optional) Validate token để trích claims trước khi tạo cookie
builder.Services.AddSingleton<TokenValidationParameters>(sp =>
{
    var jwt = sp.GetRequiredService<IConfiguration>().GetSection("Jwt");
    return new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        ValidIssuer = jwt["Issuer"],
        ValidAudience = jwt["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!)),
        ClockSkew = TimeSpan.Zero,
        RoleClaimType = System.Security.Claims.ClaimTypes.Role
    };
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}
var locOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>();
app.UseRequestLocalization(locOptions.Value);
// app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();


app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

// vào Login ngay
app.MapGet("/", () => Results.Redirect("/Admin/Account/Login"));

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Category}/{action=Index}/{id?}");

// Sử dụng localization
// Sử dụng localization



app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

Console.WriteLine("🎯 AdminWeb đang chạy...");
Console.WriteLine("📱 URL: http://localhost:5005");
Console.WriteLine("⚙️  Admin Area: http://localhost:5005/Admin/Category");
Console.WriteLine("📦 Product Area: http://localhost:5005/Admin/Product");
Console.WriteLine("🔗 API Connection: http://localhost:7245/api/");

app.Run();
