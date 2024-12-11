using System.ComponentModel.DataAnnotations;

namespace UniqloMVC.ViewModels.ResetPassword
{
    public class VerifyCodeVM
    {
        [Required(ErrorMessage = "Code is required")]
        public string Code { get; set; } = null!;
    }
}
