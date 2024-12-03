using Microsoft.AspNetCore.Identity;

namespace UniqloMVC.Models
{
    public class User : IdentityUser
    {
        public string FullName { get; set; } = null!;
        public string Passwoord { get; set; } = null!;

    }
}
