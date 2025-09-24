using API.Data;
using API.Repositories;
using API.Repositories.IRepositories;
using API.Repositories.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Controllers (enum as string nếu cần)
builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        // 🚀 Cho phép tránh vòng lặp khi serialize navigation properties
        o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;

        // 🚀 Convert enum sang string thay vì số
        o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "API V1", Version = "v1" });

    // ⚡ Thêm cấu hình Bearer
    var jwtScheme = new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Nhập: Bearer {token}"
    };

    c.AddSecurityDefinition("Bearer", jwtScheme);

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        { jwtScheme, Array.Empty<string>() }
    });
});

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
// CORS dev
builder.Services.AddCors(o => o.AddPolicy("AllowAll",
    p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

// ✅ ĐĂNG KÝ DataContext (quan trọng)
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("FlutterDB")));

// (nếu có repository thì giữ nguyên)
// Dependency Injection cho Repository
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderDetailRepository, OrderDetailRepository>();
 builder.Services.AddScoped<IUserRepository, UserRepository>();
 builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<IShippingRepository, ShippingRepository>();
builder.Services.AddScoped<ShippingService>();

// ✅ JWT (để sau dùng bảo vệ endpoint)
var jwt = builder.Configuration.GetSection("Jwt");
var keyBytes = Encoding.UTF8.GetBytes(jwt["Key"] ?? "change_me_32_chars_please");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.RequireHttpsMetadata = false; // dev
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwt["Issuer"],
            ValidAudience = jwt["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
            ClockSkew = TimeSpan.Zero
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseCors("AllowAll");


// Cấu hình static files để serve uploaded files
app.UseStaticFiles();

// Cấu hình localization
app.UseRequestLocalization();

// app.UseHttpsRedirection(); // dev http
app.UseAuthentication();
app.UseAuthorization();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
        c.RoutePrefix = "swagger";
    });
}
app.Urls.Add("https://localhost:64767");

Console.WriteLine("🚀 API Server đang chạy tại: http://localhost:7245");
Console.WriteLine("📖 Swagger UI: http://localhost:7245/swagger");
Console.WriteLine("📦 Category API: http://localhost:7245/api/category");
Console.WriteLine("📦 Product API: http://localhost:7245/api/product");

Console.WriteLine("📁 File Upload API: http://localhost:7245/api/file");

app.MapControllers();

app.Run();
