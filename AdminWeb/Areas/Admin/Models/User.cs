namespace AdminWeb.Areas.Admin.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? PasswordHash { get; set; }
        public string? Address { get; set; }
        public UserRole Role { get; set; } = UserRole.Customer;
        public DateTime CreatedAt { get; set; }

        // Navigation
        public ICollection<Order>? Orders { get; set; }
        //public ICollection<Review>? Reviews { get; set; }
    }

    public enum UserRole
    {
        Customer,
        Admin
    }
}
