using API.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

public class Shipping
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ShippingId { get; set; }

    // 👤 Thông tin người nhận
    [Required(ErrorMessage = "Tên người nhận không được để trống")]
    [StringLength(150, ErrorMessage = "Tên người nhận tối đa 150 ký tự")]
    public string RecipientName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
    [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
    [StringLength(20, ErrorMessage = "Số điện thoại tối đa 20 ký tự")]
    public string PhoneNumber { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    [StringLength(100, ErrorMessage = "Email tối đa 100 ký tự")]
    public string? Email { get; set; }

    // 📍 Địa chỉ
    [Required(ErrorMessage = "Địa chỉ giao hàng không được để trống")]
    public string Address { get; set; } = string.Empty;

    [StringLength(100, ErrorMessage = "Tên thành phố tối đa 100 ký tự")]
    public string? City { get; set; }

    [StringLength(20, ErrorMessage = "Mã bưu điện tối đa 20 ký tự")]
    public string? PostalCode { get; set; }

    // 💰 Vận chuyển
    [Range(0, 9999999999.99, ErrorMessage = "Phí vận chuyển phải lớn hơn hoặc bằng 0")]
    public decimal? ShippingFee { get; set; } = 0;   // ✅ mặc định 0

    [Range(1, 365, ErrorMessage = "Số ngày giao hàng ước tính phải từ 1 đến 365")]
    public int? EstimatedDays { get; set; } = 3;     // ✅ mặc định 3 ngày

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [JsonIgnore]
    public ICollection<Order>? Orders { get; set; }
    // ✅ Constructor tự động random PostalCode nếu null
    public Shipping()
    {
        if (string.IsNullOrEmpty(PostalCode))
        {
            PostalCode = GeneratePostalCode();
        }
    }

    private string GeneratePostalCode()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var rnd = new Random();
        string code = new string(Enumerable.Repeat(chars, 5)
            .Select(s => s[rnd.Next(s.Length)]).ToArray());
        return $"VN-{code}";
    }
}
