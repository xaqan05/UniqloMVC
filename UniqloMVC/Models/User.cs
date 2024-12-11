using Microsoft.AspNetCore.Identity;

namespace UniqloMVC.Models
{
    public class User : IdentityUser
    {
        public string FullName { get; set; } = null!;
        public string ProfileImageUrl { get; set; } = null!;
        public ICollection<Comment> Comments { get; set; }

    }
}
