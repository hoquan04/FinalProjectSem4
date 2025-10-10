
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
        // 🧩 Thêm 3 property mới cho chức năng shipper
        [Display(Name = "Ảnh CCCD mặt trước")]
        public string? CccdFrontUrl { get; set; }

        [Display(Name = "Ảnh CCCD mặt sau")]
        public string? CccdBackUrl { get; set; }

        [Display(Name = "Đang chờ duyệt Shipper")]
        public bool IsShipperRequestPending { get; set; } = false;
    }

    public class UserCreateModel
    {
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

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [StringLength(255, ErrorMessage = "Mật khẩu tối đa 255 ký tự")]
        [Display(Name = "Mật khẩu")]
        public string PasswordHash { get; set; } = string.Empty;

        [Display(Name = "Địa chỉ")]
        public string? Address { get; set; }

        [Required(ErrorMessage = "Vai trò người dùng là bắt buộc")]
        [Display(Name = "Vai trò")]
        [JsonConverter(typeof(JsonStringEnumConverter))] // 👈 Thêm để đọc enum từ API
        public UserRole Role { get; set; } = UserRole.Customer;
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
