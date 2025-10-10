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

        // ⚙️ Enum có converter để xuất ra dạng string (Customer, Admin, Shipper)
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public UserRole Role { get; set; } = UserRole.Customer;

        public DateTime? CreatedAt { get; set; } = DateTime.Now;

        [NotMapped]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? Password { get; set; }


        [JsonIgnore] public ICollection<Order>? Orders { get; set; }
        [JsonIgnore] public ICollection<Review>? Reviews { get; set; }

        // 🆕 Các trường mới cho đăng ký shipper
        public string? CccdFrontUrl { get; set; } // ảnh CCCD mặt trước
        public string? CccdBackUrl { get; set; }  // ảnh CCCD mặt sau
        public bool IsShipperRequestPending { get; set; } = false;
    }

    // ⚙️ Enum nên đặt thứ tự rõ ràng (cho DB lưu int) và đồng bộ với AdminWeb
    public enum UserRole
    {
        Customer = 0,
        Admin = 1,
        Shipper = 2
    }

}
