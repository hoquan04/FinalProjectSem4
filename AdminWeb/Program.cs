using AdminWeb.Areas.Admin.Data.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Cấu hình encoding UTF-8 cho console
Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddHttpClient("APIClient", client =>
{
    client.DefaultRequestHeaders.Accept.Add(
        new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
});

// Các service dùng HttpClient
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<OrderDetailService>();

// Đăng ký Services
builder.Services.AddHttpClient<CategoryService>();
builder.Services.AddScoped<CategoryService>();

builder.Services.AddHttpClient<ProductService>();
builder.Services.AddScoped<ProductService>();

// Cấu hình localization cho tiếng Việt
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[] { "vi-VN", "en-US" };
    options.SetDefaultCulture("vi-VN")
           .AddSupportedCultures(supportedCultures)
           .AddSupportedUICultures(supportedCultures);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Sử dụng localization
app.UseRequestLocalization();

app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Category}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

Console.WriteLine("🎯 AdminWeb đang chạy...");
Console.WriteLine("📱 URL: http://localhost:5000");
Console.WriteLine("⚙️  Admin Area: http://localhost:5000/Admin/Category");
Console.WriteLine("📦 Product Area: http://localhost:5000/Admin/Product");

app.Run();
