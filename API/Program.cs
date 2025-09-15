using API.Data;
using API.Models;
using API.Repositories;
using API.Repositories.IRepositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
    .AddEntityFrameworkStores<DataContext>().AddDefaultTokenProviders();


// Kết nối SQL Server
builder.Services.AddDbContext<DataContext>(options =>
{
    object value = options.UseSqlServer(builder.Configuration.GetConnectionString("FlutterDB"));
});



// Dependency Injection cho Repository
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();





var app = builder.Build();
// Dùng CORS đúng tên policy
app.UseCors("AllowAll");
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        c.RoutePrefix = string.Empty; // Đặt Swagger ở root ("/")
    });
}
//app.MapGet("/", () => Results.Redirect("/swagger"));
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.MapControllers();

app.Run();
//app.Run("http://0.0.0.0:7245");
