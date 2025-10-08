// File: Models/User.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace API.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        [Required, StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required, EmailAddress, StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Phone, StringLength(15)]
        public string? Phone { get; set; }

        // Lưu trong DB, không bind từ JSON, không serialize ra ngoài
        [BindNever]
        [JsonIgnore]
        [StringLength(255)]
        public string? PasswordHash { get; set; } // ❗ KHÔNG [Required], KHÔNG default ""

        public string? Address { get; set; }

        // Cho phép UI admin gửi "Admin"/"Customer" (xem lưu ý Program.cs)
        public UserRole Role { get; set; } = UserRole.Customer;

        // Dùng UTC và để non-nullable cho nhất quán
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Chỉ nhận qua body khi tạo/cập nhật mật khẩu; không map DB
        [NotMapped]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? Password { get; set; }

        // Nav props – tránh vòng lặp JSON
        [JsonIgnore] public ICollection<Order>? Orders { get; set; }
        [JsonIgnore] public ICollection<Review>? Reviews { get; set; }
    }

    public enum UserRole { Customer = 0, Admin = 1 }
}
