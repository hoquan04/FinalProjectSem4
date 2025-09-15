using API.Data;
using API.Models;
using API.Repositories;
using API.Repositories.IRepositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ================== Services ==================

// Controller
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ✅ CORS: Cho phép gọi từ mọi client
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ✅ Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<DataContext>()
    .AddDefaultTokenProviders();

// ✅ DbContext (SQL Server)
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("FlutterDB"));
});

// ✅ Dependency Injection cho Repository
builder.Services.AddScoped<IUserRepository, UserRepository>();
// builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();

// ================== App ==================
var app = builder.Build();

// ✅ Enable CORS
app.UseCors("AllowAll");

// ✅ Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
        c.RoutePrefix = "swagger"; // http://localhost:7245/swagger
    });
}

// ✅ Middleware
app.UseHttpsRedirection();
app.UseAuthentication(); // 👈 cần cho Identity
app.UseAuthorization();

app.MapControllers();

app.Run();
