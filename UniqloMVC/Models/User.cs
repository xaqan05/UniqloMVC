using Microsoft.AspNetCore.Identity;

namespace UniqloMVC.Models
{
    public class User : IdentityUser
    {
        public string FullName { get; set; } = null!;
        public string ProfileImageUrl { get; set; } = null!;
        public ICollection<Comment>? Comments { get; set; }

        public string? VerificationCode { get; set; }
        public DateTime? CodeExpiryTime { get; set; }

    }
}
