
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AdminWeb.Areas.Admin.Models
{
    public class UserViewModel
    {
        public int UserId { get; set; }

        [Display(Name = "Họ và tên")]
        public string FullName { get; set; } = string.Empty;

        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Số điện thoại")]
        public string? Phone { get; set; }

        [Display(Name = "Địa chỉ")]
        public string? Address { get; set; }

        [Display(Name = "Vai trò")]
        [JsonConverter(typeof(JsonStringEnumConverter))] // 👈 Cho phép parse từ số hoặc string
        public UserRole Role { get; set; } = UserRole.Customer;

        [Display(Name = "Ngày tạo")]
        public DateTime CreatedAt { get; set; }
    }

    public class UserCreateModel
    {
        [Required, StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required, EmailAddress, StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Phone, StringLength(15)]
        public string? Phone { get; set; }

        public string? Address { get; set; }

        [Required, MinLength(6)]
        public string Password { get; set; } = string.Empty;   // ✅ ĐÚNG KEY

        [Range(0, 1)]
        public int Role { get; set; } = 0;                     // 0=Customer, 1=Admin
    }

    public class UserEditModel
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "Họ và tên không được để trống")]
        [StringLength(100, ErrorMessage = "Họ và tên tối đa 100 ký tự")]
        [Display(Name = "Họ và tên")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [StringLength(100, ErrorMessage = "Email tối đa 100 ký tự")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [StringLength(15, ErrorMessage = "Số điện thoại tối đa 15 ký tự")]
        [Display(Name = "Số điện thoại")]
        public string? Phone { get; set; }

        [Display(Name = "Mật khẩu (để trống nếu không đổi)")]
        [StringLength(255, ErrorMessage = "Mật khẩu tối đa 255 ký tự")]
        public string PasswordHash { get; set; } = string.Empty;

        [Display(Name = "Địa chỉ")]
        public string? Address { get; set; }

        [Required(ErrorMessage = "Vai trò người dùng là bắt buộc")]
        [Display(Name = "Vai trò")]
        [JsonConverter(typeof(JsonStringEnumConverter))] // 👈
        public UserRole Role { get; set; } = UserRole.Customer;

        [Display(Name = "Ngày tạo")]
        public DateTime? CreatedAt { get; set; } = DateTime.Now;
    }
}
