using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class SignUpModel
    {
        [Required, StringLength(50)]
        public string FirstName { get; set; } = null!;

        [Required, StringLength(50)]
        public string LastName { get; set; } = null!;

        // Email đúng chuẩn, tối đa 256 ký tự
        [Required, EmailAddress, StringLength(256)]
        public string Email { get; set; } = null!;

        // Password tối thiểu 6 ký tự (tùy bạn tăng thêm rule)
        [Required, MinLength(6), StringLength(100)]
        public string Password { get; set; } = null!;

        // Phải trùng Password
        [Required, Compare("Password", ErrorMessage = "ConfirmPassword phải trùng với Password")]
        public string ConfirmPassword { get; set; } = null!;

        // Điện thoại đúng 10 chữ số (0-9)
        [Required, RegularExpression(@"^\d{10}$", ErrorMessage = "Số điện thoại phải gồm đúng 10 chữ số")]
        public string Phone { get; set; } = null!;

        [Required, StringLength(200)]
        public string Address { get; set; } = null!;
    }
}
