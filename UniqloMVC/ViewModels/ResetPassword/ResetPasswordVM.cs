using System.ComponentModel.DataAnnotations;

namespace UniqloMVC.ViewModels.ResetPassword
{
    public class ResetPasswordVM
    {
        [Required(ErrorMessage ="Email is required")]
        public string Email { get; set; } = null!;
    }
}
