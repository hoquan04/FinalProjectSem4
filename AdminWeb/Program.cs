using System.Text;
using AdminWeb.Areas.Admin.Data.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

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
    pattern: "{area:exists}/{controller=Account}/{action=Login}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
