using System.ComponentModel.DataAnnotations;

namespace AdminWeb.Areas.Admin.Models
{
    public class LoginViewModel
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterViewModel
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Address { get; set; }
    }

    public class ProfileEditViewModel
    {
        [Display(Name = "Họ tên"), Required] public string FullName { get; set; } = string.Empty;
        [Display(Name = "Email")] public string Email { get; set; } = string.Empty; // read-only
        [Display(Name = "SĐT")] public string? Phone { get; set; }
        [Display(Name = "Địa chỉ")] public string? Address { get; set; }
        [Display(Name = "Tạo lúc")] public DateTime? CreatedAt { get; set; }
    }

    public class ChangePasswordViewModel
    {
        [Display(Name = "Mật khẩu hiện tại"), Required] public string CurrentPassword { get; set; } = string.Empty;
        [Display(Name = "Mật khẩu mới"), Required, MinLength(6)] public string NewPassword { get; set; } = string.Empty;
        [Display(Name = "Nhập lại mật khẩu mới"), Required] public string ConfirmNewPassword { get; set; } = string.Empty;
    }

    public class AuthUser
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string Role { get; set; } = "Customer"; // hoặc enum string từ API
        public DateTime? CreatedAt { get; set; }
    }
}
