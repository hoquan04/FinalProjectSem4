using Microsoft.AspNetCore.Identity;

namespace API.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Fullname { get; set; } = null!;
        public string CodePasswordEmail { get; set; } = null!;
    }
}
