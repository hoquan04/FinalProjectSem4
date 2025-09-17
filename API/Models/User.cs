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

        // Lưu trong DB, không bind từ JSON, không trả ra JSON
        [BindNever]                 // ⬅️ quan trọng: model binder bỏ qua => không bị validate [Required]
        [JsonIgnore]                // không serialize ra JSON
        [Required, StringLength(255)]
        public string PasswordHash { get; set; } = string.Empty;

        public string? Address { get; set; }

        public UserRole Role { get; set; } = UserRole.Customer;

        public DateTime? CreatedAt { get; set; } = DateTime.Now;


        // Chỉ nhận vào từ body khi Register/Login, không map DB, không trả ra khi null
        [NotMapped]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? Password { get; set; }


        // Nav props – tránh vòng lặp
        [JsonIgnore] public ICollection<Order>? Orders { get; set; }
        [JsonIgnore] public ICollection<Review>? Reviews { get; set; }
    }

    public enum UserRole { Customer, Admin }
}
