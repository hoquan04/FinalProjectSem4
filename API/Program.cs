using API.Data;
using API.Models;
using API.Repositories;
using API.Repositories.IRepositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Cấu hình encoding UTF-8 cho console
Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

// Cấu hình localization cho tiếng Việt
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[] { "vi-VN", "en-US" };
    options.SetDefaultCulture("vi-VN")
           .AddSupportedCultures(supportedCultures)
           .AddSupportedUICultures(supportedCultures);
});

// CORS: Cho phép gọi từ Flutter hoặc bất kỳ client nào
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<DataContext>()
    .AddDefaultTokenProviders();

// Kết nối SQL Server
builder.Services.AddDbContext<DataContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("FlutterDB");
    Console.WriteLine($"[DbContext] Đang sử dụng chuỗi kết nối: {connectionString}");
    options.UseSqlServer(connectionString);
});

// Dependency Injection cho Repository
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();

var app = builder.Build();

// Dùng CORS đúng tên policy
app.UseCors("AllowAll");

// Cấu hình localization
app.UseRequestLocalization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Cấu hình API chạy trên port 7245
app.Urls.Add("http://localhost:7245");

Console.WriteLine("🚀 API Server đang chạy tại: http://localhost:7245");
Console.WriteLine("📖 Swagger UI: http://localhost:7245/swagger");

app.Run();
